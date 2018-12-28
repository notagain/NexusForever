CREATE TABLE IF NOT EXISTS `character_path_mission` (
    `id` BIGINT(20) UNSIGNED NOT NULL DEFAULT '0',
    `missionId` INT(8) UNSIGNED NOT NULL DEFAULT '0',
    `completed` TINYINT(1) NOT NULL DEFAULT '0',
    `progress` INT(8) UNSIGNED NOT NULL DEFAULT '0',
    `state` INT(8) UNSIGNED NOT NULL DEFAULT '0',
    PRIMARY KEY (`id`, `missionId`),
    CONSTRAINT `FK_character_mission_id__character_id` FOREIGN KEY (`id`) REFERENCES `character` (`id`) ON DELETE CASCADE
);
