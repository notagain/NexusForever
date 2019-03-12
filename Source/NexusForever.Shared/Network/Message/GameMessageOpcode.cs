namespace NexusForever.Shared.Network.Message
{
    public enum GameMessageOpcode
    {
        State                           = 0x0000,
        State2                          = 0x0001,
        ServerHello                     = 0x0003,
        Client009A                      = 0x009A, // client spell cast request, very similiar to 0x04DB, but for non-abilities -> mount, teleport, etc.
        ServerMaxCharacterLevelAchieved = 0x0036,
        ServerPlayerEnteredWorld        = 0x0061,
        ServerAuthEncrypted             = 0x0076,
        ServerCharacterLogoutStart      = 0x0092,
        ServerChangeWorld               = 0x00AD,
        ServerBuybackItemUpdated        = 0x00BA,
        ClientBuybackItemFromVendor     = 0x00BB,
        ServerBuybackItems              = 0x00BC,
        ServerBuybackItemRemoved        = 0x00BD,
        ClientVendorPurchase            = 0x00BE,
        ClientCharacterLogout           = 0x00BF,
        ClientLogout                    = 0x00C0,
        ClientHousingResidencePrivacyLevel = 0x00C9,
        ServerCostume                   = 0x00D8,
        ServerCostumeList               = 0x00D9,
        ServerCharacterCreate           = 0x00DC,
        ServerChannelUpdateLoot         = 0x00DD,
        Server00F1                      = 0x00F1, // handler sends 0x00D5 and ClientPlayerMovementSpeedUpdate
        ServerCharacterFlagsUpdated     = 0x00FE,
        Server0104                      = 0x0104, // Galactic Archive
        ServerHousingPrivacy            = 0x010E,
        ServerCharacter                 = 0x010F, // single character
        ServerItemAdd                   = 0x0111,
        ServerCharacterList             = 0x0117,
        ServerMountUnlocked             = 0x0129,
        ClientRapidTransport            = 0x0141,
        ServerItemDelete                = 0x0148,
        ClientItemDelete                = 0x0149,
        ServerCharacterSelectFail       = 0x0162,
        ClientSellItemToVendor          = 0x0166,
        ServerAbilityPoints             = 0x0169,
        ServerShowActionBar             = 0x016C,
        ServerSpellUpdate               = 0x017B,
        ClientItemSplit                 = 0x017D,
        ServerItemStackCountUpdate      = 0x017F,
        ClientItemMove                  = 0x0182,
        ClientEntitySelect              = 0x0185,
        ServerFlightPathUpdate          = 0x0188,
        ServerTitleSet                  = 0x0189,
        ServerTitleUpdate               = 0x018A,
        ServerTitles                    = 0x018B,
        Client018F                      = 0x018F,
        ServerPlayerChanged             = 0x019B,
        ServerActionSet                 = 0x019D,
        ServerStanceChanged             = 0x019F,
        ServerAbilities                 = 0x01A0,
        Server01A3                      = 0x01A3, // AMP
        ServerPathUpdateXP              = 0x01AA,
        ServerExperienceGained          = 0x01AC,
        ServerChatJoin                  = 0x01BC,
        ServerChatAccept                = 0x01C2,
        ClientChat                      = 0x01C3,
        ServerChat                      = 0x01C8,
        ClientChatWhisper               = 0x01D4,
        Server0237                      = 0x0237, // UI related, opens or closes different UI windows (bank, barber, ect...)
        ClientPing                      = 0x0241,
        ClientEncrypted                 = 0x0244,
        ServerCombatLog                 = 0x0247,
        ClientCostumeSave               = 0x0255,
        ClientCostumeSet                = 0x0256,
        ServerCostumeSave               = 0x0257,
        ServerCostumeItemUnlockMultiple = 0x0258,
        ServerCostumeItemUnlock         = 0x0259,
        ServerCostumeItemList           = 0x025A,
        ClientCharacterCreate           = 0x025B,
        ClientPacked                    = 0x025C, // the same as ClientEncrypted except the contents isn't encrypted?
        ServerPlayerCreate              = 0x025E,
        ServerEntityCreate              = 0x0262,
        ClientCharacterDelete           = 0x0352,
        ServerEntityDestory             = 0x0355,
        ServerQuestInit                 = 0x035F,
        ClientEmote                     = 0x037E,
        ClientCostumeItemForget         = 0x038B,
        Server03AA                      = 0x03AA, // friendship account related
        Server03BE                      = 0x03BE, // friendship related
        ServerRealmInfo                 = 0x03DB,
        ServerRealmEncrypted            = 0x03DC,
        ClientCheat                     = 0x03E0,
        Server0497                      = 0x0497, // guild info
        ClientCastSpell                 = 0x04DB,
        ServerHousingResidenceDecor     = 0x04DE,
        ServerHousingProperties         = 0x04DF,
        ServerHousingPlots              = 0x04E1,
        ClientHousingCrateAllDecor      = 0x04EC,
        ServerHousingNeighbors          = 0x0507,
        ServerHousingVendorList         = 0x0508,
        ClientHousingRemodel            = 0x050A,
        ClientHousingDecorUpdate        = 0x050B,
        ClientHousingPlugUpdate         = 0x0510,
        ClientHousingVendorList         = 0x0525,
        ClientHousingRenameProperty     = 0x0529,
        ClientHousingVisit              = 0x0531,
        ClientHousingEditMode           = 0x053C,
        ServerSpellList                 = 0x0551,
        ServerItemSwap                  = 0x0568,
        ServerItemMove                  = 0x0569,
        Server0570                      = 0x0570, // keybind related
        ClientHelloRealm                = 0x058F,
        ServerAuthAccepted              = 0x0591,
        ClientHelloAuth                 = 0x0592,
        ServerClientLogout              = 0x0594,
        ServerAvailableMail             = 0x05A3,
        Server0635                      = 0x0635,
        ServerMovementControl           = 0x0636, // handler sends 0x0635 and 0x063A
        ClientEntityCommand             = 0x0637, // bidirectional? packet has both read and write handlers 
        ServerEntityCommand             = 0x0638, // bidirectional? packet has both read and write handlers
        Server0639                      = 0x0639, // mount up or something
        ClientPlayerMovementSpeedUpdate = 0x063B,
        ServerOwnedCommodityOrders      = 0x064C,
        ServerOwnedItemAuctions         = 0x064D,
        ClientPathActivate              = 0x06B2,
        ServerPathActivateResult        = 0x06B3,
        ServerPathRefresh               = 0x06B4,
        ServerPathEpisodeProgress       = 0x06B5,
        Server06B6                      = 0x06B6,
        Server06B7                      = 0x06B7,
        Server06B8                      = 0x06B8,
        Server06B9                      = 0x06B9,
        ServerPathMissionActivate       = 0x06BA, 
        ServerPathMissionUpdate         = 0x06BB,
        ServerPathLog                   = 0x06BC,
        ClientPathUnlock                = 0x06BD,
        ServerPathUnlockResult          = 0x06BE,
        ServerPathCurrentEpisode        = 0x06BF,
        ServerRealmList                 = 0x0761, // bidirectional? packet has both read and write handlers
        ServerRealmMessages             = 0x0763,
        ClientTitleSet                  = 0x078E,
        ClientRealmList                 = 0x07A4,
        ClientCharacterList             = 0x07E0,
        ClientVendor                    = 0x07EA,
        ClientCharacterSelect           = 0x07DD,
        ServerSpellGo                   = 0x07F4,
        Server07F5                      = 0x07F5, // spell related
        Server07F6                      = 0x07F6, // spell related
        Server07F7                      = 0x07F7, // spell related
        Server07F8                      = 0x07F8, // spell related
        Server07F9                      = 0x07F9, // spell related
        Server07FA                      = 0x07FA, // spell related
        Server07FB                      = 0x07FB, // spell miss info?
        ServerSpellCastResult           = 0x07FC,
        Server07FD                      = 0x07FD, // spell related
        Server07FE                      = 0x07FE, // spell related
        ServerSpellStart                = 0x07FF,
        ServerCooldown                  = 0x0804,
        Server0811                      = 0x0811, // spell related: broadcast parts of 0x07FF?
        Server0814                      = 0x0814, // spell related
        Server0816                      = 0x0816, // spell related: broadcast parts of 0x07FF?
        Server0817                      = 0x0817, // spell related
        ClientStorefrontRequestCatalog  = 0x082D,
        Server0854                      = 0x0854, // crafting schematic
        Server0856                      = 0x0856, // tradeskills
        Server086F                      = 0x086F,
        Server08B3                      = 0x08B3,
        ServerSetUnitPathType           = 0x08B8,
        Server0905                      = 0x0905, // displays addons on mounts, maybe more
        Server0908                      = 0x0908,
        ServerVendorItemsUpdated        = 0x090B,
        ClientCostumeItemUnlock         = 0x090F,
        ServerPlayerCurrencyChanged     = 0x0919,
        ServerCooldownList              = 0x091B,
        Server092C                      = 0x092C,
        ServerItemVisualUpdate          = 0x0933,
        ServerEntityStatUpdate          = 0x0935, // 0x0938??
        Server0934                      = 0x0934,
        Server093A                      = 0x093A,
        ServerEmote                     = 0x093C,
        ClientWhoRequest                = 0x0959,
        ServerWhoResponse               = 0x095A,
        ServerGrantAccountCurrency      = 0x0967,
        ServerAccountEntitlements       = 0x0968,
        ServerGenericUnlockList         = 0x0981,
        ServerGenericUnlock             = 0x0982,
        ServerGenericUnlockResult       = 0x0985
    }
}
