﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using NexusForever.Shared;
using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network;
using NexusForever.WorldServer.Database;
using NexusForever.WorldServer.Database.Character.Model;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NexusForever.WorldServer.Network.Message.Model.Shared;
using NLog;

namespace NexusForever.WorldServer.Game.Entity
{
    public class Inventory : IUpdate, IEnumerable<Bag>, ISaveCharacter
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private static ulong ItemLocationToDragDropData(InventoryLocation location, ushort slot)
        {
            // TODO: research this more, client version of this is more complex
            return ((ulong)location << 8) | slot;
        }

        private readonly ulong characterId;
        private readonly Player player;
        private readonly Dictionary<InventoryLocation, Bag> bags = new Dictionary<InventoryLocation, Bag>();
        private readonly List<Item> deletedItems = new List<Item>();

        /// <summary>
        /// Create a new <see cref="Inventory"/> from <see cref="Player"/> database model.
        /// </summary>
        public Inventory(Player owner, Character model)
        {
            characterId = owner?.CharacterId ?? 0ul;
            player      = owner;

            foreach ((InventoryLocation location, uint defaultCapacity) in AssetManager.InventoryLocationCapacities)
                bags.Add(location, new Bag(location, defaultCapacity));

            foreach (var itemModel in model.Item.Select(i => i).OrderBy(i => i.Location).ToList())
            {
                Item item = new Item(itemModel);
                AddItem(item);

                if (IsEquippedBagLocation(item.Location, item.BagIndex))
                    GetBag(InventoryLocation.Inventory).Resize((int)item.Entry.MaxStackCount);
            }
        }

        /// <summary>
        /// Create a new <see cref="Inventory"/> from supplied <see cref="CharacterCreationEntry"/>.
        /// </summary>
        public Inventory(ulong owner, CharacterCreationEntry creationEntry)
        {
            characterId = owner;

            foreach ((InventoryLocation location, uint defaultCapacity) in AssetManager.InventoryLocationCapacities)
                bags.Add(location, new Bag(location, defaultCapacity));

            foreach (uint itemId in creationEntry.ItemIds.Where(i => i != 0u))
            {
                Item2Entry itemEntry = GameTableManager.Item.GetEntry(itemId);
                if (itemEntry == null)
                    throw new ArgumentNullException();

                Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(itemEntry.Item2TypeId);
                if (typeEntry.ItemSlotId == 0)
                    ItemCreate(itemEntry, 1u);
                else
                    ItemCreate(itemEntry);
            }
        }

        public void Update(double lastTick)
        {
            // TODO: tick items with limited lifespans
        }

        public void Save(CharacterContext context)
        {
            foreach (Item item in bags.Values
                .Where(b => b.Location != InventoryLocation.Ability)
                .SelectMany(i => i))
                item.Save(context);

            foreach (Item item in deletedItems)
                item.Save(context);
            deletedItems.Clear();
        }

        /// <summary>
        /// Returns <see cref="ItemVisual"/> for any visible items.
        /// </summary>
        public IEnumerable<ItemVisual> GetItemVisuals(Costume costume)
        {
            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);

            foreach (Item item in bag)
            {
                if (!IsVisualItem(item))
                    continue;

                Item2TypeEntry itemTypeEntry = GameTableManager.ItemType.GetEntry(item.Entry.Item2TypeId);

                ItemVisual visual = GetItemVisual((ItemSlot)itemTypeEntry.ItemSlotId, costume);
                if (visual != null)
                    yield return visual;
            }
        }

        /// <summary>
        /// Returns <see cref="ItemVisual"/> for supplied <see cref="ItemSlot"/>.
        /// </summary>
        private ItemVisual GetItemVisual(ItemSlot itemSlot, Costume costume)
        {
            ImmutableList<EquippedItem> indexes = AssetManager.GetEquippedBagIndexes(itemSlot);
            if (indexes == null || indexes.Count != 1)
                throw new ArgumentOutOfRangeException();

            EquippedItem index = indexes[0];

            CostumeItem costumeItem = null;
            if (costume != null)
            {
                if (index == EquippedItem.WeaponPrimary)
                    costumeItem = costume.GetItem(CostumeItemSlot.Weapon);
                else if (index >= EquippedItem.Chest && index <= EquippedItem.Hands)
                {
                    // skip any slot that is hidden
                    if ((costume.Mask & (1 << (int)index)) == 0)
                        return new ItemVisual
                        {
                            Slot = itemSlot
                        };

                    costumeItem = costume.GetItem((CostumeItemSlot)index);
                }
            }

            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);
            Item item = bag.GetItem((uint)index);

            return new ItemVisual
            {
                Slot      = itemSlot,
                DisplayId = Item.GetDisplayId(costumeItem != null ? costumeItem.Entry : item?.Entry),
                DyeData   = costumeItem?.DyeData ?? 0
            };
        }

        /// <summary>
        /// Update <see cref="ItemVisual"/> and broadcast <see cref="ServerItemVisualUpdate"/> for optional supplied <see cref="Costume"/>.
        /// </summary>
        public void VisualUpdate(Costume costume)
        {
            var itemVisualUpdate = new ServerItemVisualUpdate
            {
                Guid = player.Guid
            };

            itemVisualUpdate.ItemVisuals.Add(VisualUpdate(ItemSlot.WeaponPrimary, costume));
            for (ItemSlot index = ItemSlot.ArmorChest; index <= ItemSlot.ArmorHands; index++)
                itemVisualUpdate.ItemVisuals.Add(VisualUpdate(index, costume));

            if (!player.IsLoading)
                player.EnqueueToVisible(itemVisualUpdate, true);
        }

        /// <summary>
        /// Update <see cref="ItemVisual"/> and broadcast <see cref="ServerItemVisualUpdate"/> for supplied <see cref="Item"/>
        /// </summary>
        private void VisualUpdate(Item item)
        {
            if (item == null)
                throw new ArgumentNullException();

            var itemVisualUpdate = new ServerItemVisualUpdate
            {
                Guid = player.Guid
            };

            Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(item.Entry.Item2TypeId);

            Costume costume = null;
            if (player.CostumeIndex >= 0)
                costume = player.CostumeManager.GetCostume((byte)player.CostumeIndex);

            itemVisualUpdate.ItemVisuals.Add(VisualUpdate((ItemSlot)typeEntry.ItemSlotId, costume));

            if (!player.IsLoading)
                player.EnqueueToVisible(itemVisualUpdate, true);
        }

        /// <summary>
        /// Update visual for supplied <see cref="ItemSlot"/> and optional <see cref="Costume"/>.
        /// </summary>
        private ItemVisual VisualUpdate(ItemSlot slot, Costume costume)
        {
            ItemVisual visual = GetItemVisual(slot, costume);
            player?.SetAppearance(visual);
            return visual;
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available <see cref="EquippedItem"/> bag index.
        /// </summary>
        public void ItemCreate(uint itemId)
        {
            Item2Entry itemEntry = GameTableManager.Item.GetEntry(itemId);
            if (itemEntry == null)
                throw new ArgumentNullException();

            ItemCreate(itemEntry);
        }

        /// <summary>
        /// Create a new <see cref="Item"/> from supplied <see cref="Spell4BaseEntry"/> in the first available <see cref="InventoryLocation.Ability"/> bag slot.
        /// </summary>
        public Item SpellCreate(Spell4BaseEntry spell4BaseEntry, byte reason)
        {
            if (spell4BaseEntry == null)
                throw new ArgumentNullException();

            Bag bag = GetBag(InventoryLocation.Ability);
            Debug.Assert(bag != null);

            uint bagIndex = bag.GetFirstAvailableBagIndex();
            if (bagIndex == uint.MaxValue)
                return null;

            var spell = new Item(characterId, spell4BaseEntry);
            AddItem(spell, InventoryLocation.Ability, bagIndex);

            if (!player?.IsLoading ?? false)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item   = spell.BuildNetworkItem(),
                        Reason = reason
                    }
                });
            }

            return spell;
        }

        /// <summary>
        /// Add <see cref="Item"/> in the first available bag index for the given <see cref="InventoryLocation"/> .
        /// </summary>
        public void AddItem(Item item, InventoryLocation inventoryLocation)
        {
            Bag bag = GetBag(inventoryLocation);
            uint bagIndex = bag.GetFirstAvailableBagIndex();

            if (bagIndex == uint.MaxValue)
            {
                throw new ArgumentException($"InventoryLocation {inventoryLocation} is full!");
            }
            
            // Stacks are bought back in full, so no need to worry about splitting stacks
            AddItem(item, inventoryLocation, bagIndex);

            if (!player?.IsLoading ?? false)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item = item.BuildNetworkItem(),
                        Reason = 49
                    }
                });
            }
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available <see cref="EquippedItem"/> bag index.
        /// </summary>
        public void ItemCreate(Item2Entry itemEntry)
        {
            if (itemEntry == null)
                throw new ArgumentNullException();

            Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(itemEntry.Item2TypeId);
            if (typeEntry.ItemSlotId == 0)
                throw new ArgumentException($"Item {itemEntry.Id} isn't equippable!");

            Bag bag = GetBag(InventoryLocation.Equipped);
            Debug.Assert(bag != null);

            // find first free bag index, some items can be equipped into multiple slots
            foreach (uint bagIndex in AssetManager.GetEquippedBagIndexes((ItemSlot)typeEntry.ItemSlotId))
            {
                if (bag.GetItem(bagIndex) != null)
                    continue;

                Item item = new Item(characterId, itemEntry);
                AddItem(item, InventoryLocation.Equipped, bagIndex);
                break;
            }
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(uint itemId, uint count, byte reason = 49, uint charges = 0)
        {
            Item2Entry itemEntry = GameTableManager.Item.GetEntry(itemId);
            if (itemEntry == null)
                throw new ArgumentNullException();

            ItemCreate(itemEntry, count, reason, charges);
        }

        /// <summary>
        /// Create a new <see cref="Item"/> in the first available inventory bag index or stack.
        /// </summary>
        public void ItemCreate(Item2Entry itemEntry, uint count, byte reason = 49, uint charges = 0)
        {
            if (itemEntry == null)
                throw new ArgumentNullException();

            Bag bag = GetBag(InventoryLocation.Inventory);
            Debug.Assert(bag != null);

            // update any existing stacks before creating new items
            if (itemEntry.MaxStackCount > 1)
            {
                foreach (Item item in bag.Where(i => i.Entry.Id == itemEntry.Id))
                {
                    if (count == 0u)
                        break;

                    if (item.StackCount == itemEntry.MaxStackCount)
                        continue;

                    uint newStackCount = Math.Min(item.StackCount + count, itemEntry.MaxStackCount);
                    count -= newStackCount - item.StackCount;
                    ItemStackCountUpdate(item, newStackCount);
                }
            }

            // create new stacks for the remaining count
            while (count > 0)
            {
                uint bagIndex = bag.GetFirstAvailableBagIndex();
                if (bagIndex == uint.MaxValue)
                    return;

                var item = new Item(characterId, itemEntry, Math.Min(count, itemEntry.MaxStackCount), charges);
                AddItem(item, InventoryLocation.Inventory, bagIndex);

                if (!player?.IsLoading ?? false)
                {
                    player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                    {
                        InventoryItem = new InventoryItem
                        {
                            Item = item.BuildNetworkItem(),
                            Reason = reason
                        }
                    });
                }

                count -= item.StackCount;
            }
        }

        /// <summary>
        /// Move <see cref="Item"/> from one <see cref="ItemLocation"/> to another, this is called directly from a packet hander.
        /// </summary>
        public void ItemMove(ItemLocation from, ItemLocation to)
        {
            Bag srcBag = GetBag(from.Location);
            if (srcBag == null)
                throw new InvalidPacketValueException();

            Item srcItem = srcBag.GetItem(from.BagIndex);
            if (srcItem == null)
                throw new InvalidPacketValueException();

            if (!IsValidLocationForItem(srcItem, to.Location, to.BagIndex))
                throw new InvalidPacketValueException();

            Bag dstBag = GetBag(to.Location);
            if (dstBag == null)
                throw new InvalidPacketValueException();

            Item dstItem = dstBag.GetItem(to.BagIndex);

            if (IsEquippedBagLocation(from) || IsEquippedBagLocation(to))
                HandleEquippedBagChange(from, to, srcItem, dstItem);
            else
                ItemMove(from, to, srcItem, dstItem);
        }

        /// <summary>
        /// Move <see cref="Item"/> from one <see cref="ItemLocation"/> to another, handling swapping if necessary.
        /// </summary>
        private void ItemMove(ItemLocation from, ItemLocation to, Item srcItem, Item dstItem = null)
        {
            if (srcItem == null || from == null || to == null)
                throw new InvalidPacketValueException();

            if (dstItem == null && GetBag(to.Location).GetItem(to.BagIndex) != null)
                throw new InvalidPacketValueException();

            try
            {
                RemoveItem(srcItem);

                if (dstItem == null)
                {
                    // no item at destination, just a simple move
                    RemoveItem(srcItem);
                    AddItem(srcItem, to.Location, to.BagIndex);

                    player.Session.EnqueueMessageEncrypted(new ServerItemMove
                    {
                        To = new ItemDragDrop
                        {
                            Guid = srcItem.Guid,
                            DragDrop = ItemLocationToDragDropData(to.Location, (ushort)to.BagIndex)
                        }
                    });
                }
                else if (srcItem.Entry.Id == dstItem.Entry.Id)
                {
                    // item at destination with same entry, try and stack
                    uint newStackCount = Math.Min(dstItem.StackCount + srcItem.StackCount, dstItem.Entry.MaxStackCount);
                    uint oldStackCount = srcItem.StackCount - (newStackCount - dstItem.StackCount);
                    ItemStackCountUpdate(dstItem, newStackCount);

                    if (oldStackCount == 0u)
                    {
                        ItemDelete(new ItemLocation
                        {
                            Location = srcItem.Location,
                            BagIndex = srcItem.BagIndex
                        });
                    }  
                    else
                        ItemStackCountUpdate(srcItem, oldStackCount);
                }
                else
                {
                    // item at destination, swap with source item
                    RemoveItem(srcItem);
                    RemoveItem(dstItem);
                    AddItem(srcItem, to.Location, to.BagIndex);
                    AddItem(dstItem, from.Location, from.BagIndex);

                    player.Session.EnqueueMessageEncrypted(new ServerItemSwap
                    {
                        To = new ItemDragDrop
                        {
                            Guid = srcItem.Guid,
                            DragDrop = ItemLocationToDragDropData(to.Location, (ushort)to.BagIndex)
                        },
                        From = new ItemDragDrop
                        {
                            Guid = dstItem.Guid,
                            DragDrop = ItemLocationToDragDropData(from.Location, (ushort)from.BagIndex)
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                // TODO: rollback
                log.Fatal(exception);
            }
        }

        /// Ensures any containers being moved are empty of items
        /// </summary>
        public bool CheckInventoryContainersOnMove(Item srcItem, Item dstItem)
        {
            if (IsInventoryContainer(srcItem) || (dstItem != null && IsInventoryContainer(dstItem)))
            {
                var containerToCheck = srcItem;
                if (dstItem != null && IsInventoryContainer(dstItem))
                    containerToCheck = dstItem;

                GetInventorySlotsForContainer((EquippedItem)containerToCheck.BagIndex, out uint[] inventorySlots);
                if (inventorySlots != null)
                {
                    foreach (uint slot in inventorySlots)
                    {
                        Bag bag = GetBag(InventoryLocation.Inventory);
                        if (bag == null)
                            throw new InvalidPacketValueException();

                        Item item = bag.GetItem(slot);
                        if (item != null)
                            return false;
                    }
                }
            }

            return true;
        }
        
        /// <summary>
        /// Split a subset of <see cref="Item"/> to create a new <see cref="Item"/> of split amount
        /// </summary>
        public void ItemSplit(ulong itemGuid, ItemLocation newItemLocation, uint count)
        {
            Item item = GetItem(itemGuid);
            if (item == null)
                throw new InvalidPacketValueException();

            if (item.Entry.MaxStackCount <= 1u)
                throw new InvalidPacketValueException();

            if (count >= item.StackCount)
                throw new InvalidPacketValueException();

            Bag dstBag = GetBag(newItemLocation.Location);
            if (dstBag == null)
                throw new InvalidPacketValueException();

            Item dstItem = dstBag.GetItem(newItemLocation.BagIndex);
            if (dstItem != null)
                throw new InvalidPacketValueException();

            var newItem = new Item(characterId, item.Entry, Math.Min(count, item.Entry.MaxStackCount));
            AddItem(newItem, newItemLocation.Location, newItemLocation.BagIndex);

            if (!player?.IsLoading ?? false)
            {
                player.Session.EnqueueMessageEncrypted(new ServerItemAdd
                {
                    InventoryItem = new InventoryItem
                    {
                        Item = newItem.BuildNetworkItem(),
                        Reason = 49
                    }
                });
            }

            ItemStackCountUpdate(item, item.StackCount - count);
        }

        /// <summary>
        /// Return <see cref="Item"/> at supplied <see cref="ItemLocation"/>.
        /// </summary>
        public Item GetItem(ItemLocation itemLocation)
        {
            return GetItem(itemLocation.Location, itemLocation.BagIndex);
        }

        /// <summary>
        /// Return <see cref="Item"/> at supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        public Item GetItem(InventoryLocation location, uint bagIndex)
        {
            Bag bag = GetBag(location);
            if (bag == null)
                throw new InvalidPacketValueException();

            Item item = bag.GetItem(bagIndex);
            if (item == null)
                throw new InvalidPacketValueException();

            return item;
        }

        /// <summary>
        /// Return <see cref="Item"/> with supplied guid.
        /// </summary>
        public Item GetItem(ulong guid)
        {
            foreach (Bag bag in bags.Values)
                foreach (Item item in bag)
                    if (item.Guid == guid)
                        return item;

            return null;
        }

        public Item GetSpell(Spell4BaseEntry spell4BaseEntry)
        {
            Bag bag = GetBag(InventoryLocation.Ability);
            foreach (Item item in bag)
                if (item.SpellEntry == spell4BaseEntry)
                    return item;

            return null;
        }

        /// <summary>
        /// Delete <see cref="Item"/> at supplied <see cref="ItemLocation"/>, this is called directly from a packet hander.
        /// </summary>
        public Item ItemDelete(ItemLocation from)
        {
            Bag srcBag = GetBag(from.Location);
            if (srcBag == null)
                throw new InvalidPacketValueException();

            Item srcItem = srcBag.GetItem(from.BagIndex);
            if (srcItem == null)
                throw new InvalidPacketValueException();

            srcBag.RemoveItem(srcItem);
            srcItem.EnqueueDelete();
            deletedItems.Add(srcItem);

            player.Session.EnqueueMessageEncrypted(new ServerItemDelete
            {
                Guid = srcItem.Guid
            });

            return srcItem;
        }

        /// <summary>
        /// Checks if supplied <see cref="InventoryLocation"/> and bag index valid for <see cref="Item"/>.
        /// </summary>
        private bool IsValidLocationForItem(Item item, InventoryLocation location, uint bagIndex)
        {
            Bag bag = GetBag(location);
            if (bag == null)
                return false;

            if (location == InventoryLocation.Equipped)
            {
                Item2TypeEntry typeEntry = GameTableManager.ItemType.GetEntry(item.Entry.Item2TypeId);
                if (typeEntry.ItemSlotId == 0)
                    return false;

                ImmutableList<EquippedItem> bagIndexes = AssetManager.GetEquippedBagIndexes((ItemSlot)typeEntry.ItemSlotId);
                if (bagIndexes.All(i => i != (EquippedItem) bagIndex))
                    return false;

                /*if (owner.Character.Class != item.Entry.ClassRequired)
                    return false;

                if (owner.Character.Race != item.Entry.RaceRequired)
                    return false;*/
            }

            return true;
        }

        /// <summary>
        /// Add <see cref="Item"/> to the supplied <see cref="InventoryLocation"/> and bag index.
        /// </summary>
        private void AddItem(Item item, InventoryLocation location, uint bagIndex)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location != InventoryLocation.None)
                throw new ArgumentException();

            item.Location = location;
            item.BagIndex = bagIndex;

            AddItem(item);
        }

        private void AddItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location == InventoryLocation.None)
                throw new ArgumentException();

            Bag bag = GetBag(item.Location);
            if (bag == null)
                throw new ArgumentNullException();

            bag.AddItem(item);

            if (player != null && bag.Location == InventoryLocation.Equipped)
                if (IsVisualItem(item))
                    VisualUpdate(item);
        }

        /// <summary>
        /// Remove <see cref="Item"/> from it's current <see cref="InventoryLocation"/> and bag slot.
        /// </summary>
        private void RemoveItem(Item item)
        {
            if (item == null)
                throw new ArgumentNullException();
            if (item.Location == InventoryLocation.None)
                throw new ArgumentException();

            Bag bag = GetBag(item.Location);
            Debug.Assert(bag != null);

            if (player != null && bag.Location == InventoryLocation.Equipped)
                if (IsVisualItem(item))
                    VisualUpdate(item);

            bag.RemoveItem(item);
        }

        /// <summary>
        /// Update <see cref="Item"/> with supplied stack count.
        /// </summary>
        private void ItemStackCountUpdate(Item item, uint stackCount)
        {
            if (item == null)
                throw new ArgumentNullException();

            item.StackCount = stackCount;

            player.Session.EnqueueMessageEncrypted(new ServerItemStackCountUpdate
            {
                Guid       = item.Guid,
                StackCount = stackCount,
                Reason     = 0
            });
        }

        /// <summary>
        /// Apply stack updates and deletion to <see cref="Item"/> on use
        /// </summary>
        public bool ItemUse(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item), "Item is null.");

            // This should only apply for re-usable items, like Quest Clickies.
            if (item.Entry.MaxCharges == 0 && item.Entry.MaxStackCount == 1)
                return true;

            if ((item.Charges <= 0 && item.Entry.MaxCharges > 1)|| (item.StackCount <= 0 && item.Entry.MaxStackCount > 1))
                return false;

            if(item.Charges >= 1 && item.Entry.MaxStackCount == 1)
                item.Charges--;

            if (item.Entry.MaxStackCount > 1 && item.StackCount > 0)
                ItemStackCountUpdate(item, item.StackCount - 1);

            // TODO: Set Deletion reason to 1, when consuming a single charge item.
            if ((item.StackCount == 0 && item.Entry.MaxStackCount > 1) || (item.Charges == 0 && item.Entry.MaxCharges > 0))
            {
                ItemDelete(new ItemLocation
                {
                    Location = item.Location,
                    BagIndex = item.BagIndex
                });
            }

            return true;
        }

        /// Check if the current <see cref="Item"/> is in a visual slot
        /// </summary>
        private bool IsVisualItem(Item item)
        {
            EquippedItem[] visualItems = new EquippedItem[]
            {EquippedItem.Chest, EquippedItem.Head, EquippedItem.Legs, EquippedItem.Hands, EquippedItem.WeaponPrimary, EquippedItem.Shoulder, EquippedItem.Feet};

            return visualItems.Contains((EquippedItem)item.BagIndex);
        }

        /// <summary>
        /// Handles the addition, removal, and swapping of equipped bags. Should only be called from <see cref="ItemMove(ItemLocation, ItemLocation)"/>
        /// </summary>
        private void HandleEquippedBagChange(ItemLocation from, ItemLocation to, Item srcItem, Item dstItem)
        {
            Bag inventory = GetBag(InventoryLocation.Inventory);
            if (inventory == null)
                throw new InvalidPacketValueException($"Can't find Inventory.");

            ItemError itemError = 0;

            if (!srcItem.IsEquippableBag() && !IsEquippedBagLocation(to))
                itemError = ItemError.InvalidForThisSlot;

            // TODO: Mail items it can't bag, instead of erroring? Or, flow into overflow?
            int capacityChange = 0;
            if (srcItem.IsEquippableBag() && IsEquippedBagLocation(to))
            {
                if (dstItem == null && !IsEquippedBagLocation(from))
                    capacityChange = (int)srcItem.Entry.MaxStackCount; // Adding bag
                else if (dstItem != null && !IsEquippedBagLocation(from))
                    capacityChange = (int)srcItem.Entry.MaxStackCount - (int)dstItem.Entry.MaxStackCount; // Replacing bag
            }
            else if (srcItem.IsEquippableBag() && IsEquippedBagLocation(from))
            {
                if (dstItem == null && !IsEquippedBagLocation(to))
                    capacityChange -= (int)srcItem.Entry.MaxStackCount; // Removing bag (-1 for actual bag)
                else if (dstItem != null && dstItem.IsEquippableBag())
                    capacityChange = (int)dstItem.Entry.MaxStackCount - (int)srcItem.Entry.MaxStackCount; // Replacing bag
            }

            if (capacityChange < 0 && inventory.SlotsRemaining < (capacityChange * -1 + 1))
                itemError = ItemError.BagMustBeEmpty;

            if (itemError == 0)
            {
                ItemMove(from, to, srcItem, dstItem);

                if (capacityChange < 0)
                    MoveItemsAfterBagSwap(capacityChange);
                    
                if (capacityChange != 0)
                    inventory.Resize(capacityChange);
            }
            else
                player.Session.EnqueueMessageEncrypted(new ServerItemError
                {
                    ItemGuid = srcItem.Guid,
                    ErrorCode = itemError
                });
        }

        /// <summary>
        /// Used to adjust items after a bag is equipped. Should only occur when the Inventory size shrinks.
        /// </summary>
        private void MoveItemsAfterBagSwap(int capacityChange)
        {
            Bag inventory = GetBag(InventoryLocation.Inventory);
            if (inventory == null)
                throw new InvalidPacketValueException("Can't find Inventory.");

            int inventoryMaxIndex = inventory.GetSize() - 1;

            for (int i = inventoryMaxIndex; i > (inventoryMaxIndex + capacityChange); i--)
            {
                Item item = inventory.GetItem((uint)i);
                if (item != null)
                {
                    uint newItemIndex = inventory.GetFirstAvailableBagIndex();
                    if (newItemIndex == uint.MaxValue)
                        throw new InvalidPacketValueException($"Missing BagIndex to move item {item.Guid} to.");

                    ItemMove(new ItemLocation
                    {
                        Location = item.Location,
                        BagIndex = item.BagIndex
                    },
                    new ItemLocation
                    {
                        Location = InventoryLocation.Inventory,
                        BagIndex = newItemIndex
                    },
                    item,
                    null);
                }
            }
        }

        /// <summary>
        /// Check if the <see cref="Item"/> is an equipped container
        /// </summary>
        private bool IsEquippedBag(Item item)
        {
            EquippedItem[] containers = new EquippedItem[]
            {EquippedItem.Bag0, EquippedItem.Bag1, EquippedItem.Bag2, EquippedItem.Bag3};

            return containers.Contains((EquippedItem)item.BagIndex);
        }

        /// <summary>
        /// Returns whether or not the provided <see cref="ItemLocation"/> is an Equipped Bag slot.
        /// </summary>
        private bool IsEquippedBagLocation(ItemLocation itemLocation)
        {
            return IsEquippedBagLocation(itemLocation.Location, itemLocation.BagIndex);
        }

        /// <summary>
        /// Returns whether or not the provided <see cref="InventoryLocation"/> and bagIndex match an Equipped Bag slot.
        /// </summary>
        private bool IsEquippedBagLocation(InventoryLocation inventoryLocation, uint bagIndex)
        {
            EquippedItem[] containers = new EquippedItem[]
            {EquippedItem.Bag0, EquippedItem.Bag1, EquippedItem.Bag2, EquippedItem.Bag3};

            if (inventoryLocation != InventoryLocation.Equipped)
                return false;

            return containers.Contains((EquippedItem)bagIndex);
        }

        private Bag GetBag(InventoryLocation location)
        {
            return bags.TryGetValue(location, out Bag container) ? container : null;
        }

        public IEnumerator<Bag> GetEnumerator()
        {
            return bags.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
