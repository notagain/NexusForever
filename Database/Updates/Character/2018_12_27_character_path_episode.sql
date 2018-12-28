CREATE TABLE IF NOT EXISTS `character_path_episode` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `episodeId` INT(8) UNSIGNED NOT NULL DEFAULT '0',
    `rewardReceived` TINYINT(1) NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `episodeId`),
    CONSTRAINT `FK_character_episode_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
