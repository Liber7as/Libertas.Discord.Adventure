using Libertas.Discord.Adventure.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Libertas.Discord.Adventure.Data.Configurations;

public partial class MobPresetConfiguration
{
    private static void HasData(EntityTypeBuilder<MobPreset> builder)
    {
        var idIndex = 0;

        builder.HasData(
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Abyssal Demon",
                ImageUrl = "https://www.maxpixel.net/static/photo/1x/Fire-Demon-Flames-Chaos-Burn-Inflamed-Devil-Hell-2708544.jpg",
                MinHealth = 17,
                MaxHealth = 32,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Abyssal Drake",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/01/21/06/04/dragon-3095910_960_720.png",
                MinHealth = 62,
                MaxHealth = 77,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Aether Dragon",
                ImageUrl = "https://vignette.wikia.nocookie.net/fairytailfanon/images/8/83/Beltazlel_HS.jpg",
                MinHealth = 41,
                MaxHealth = 56,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Aether Faerie",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/03/22/11/02/fairy-2164638_960_720.png",
                MinHealth = 19,
                MaxHealth = 34,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Aether Wolf",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/11/18/21/10/wolf-1836875_960_720.jpg",
                MinHealth = 15,
                MaxHealth = 30,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Air Elemental",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/07/18/49/priestess-1722370_960_720.jpg",
                MinHealth = 34,
                MaxHealth = 49,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ancient Earth Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/05/26/21/32/control-785555_960_720.jpg",
                MinHealth = 32,
                MaxHealth = 47,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ancient Spellcaster",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/04/13/01/53/man-4123703_960_720.jpg",
                MinHealth = 36,
                MaxHealth = 51,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ancient Tree Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/03/08/20/12/tree-2127699_960_720.jpg",
                MinHealth = 30,
                MaxHealth = 45,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Arcane Construct",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/04/16/21/49/deer-1333814_960_720.jpg",
                MinHealth = 7,
                MaxHealth = 22,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Arcane-Touched Satyr",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/05/22/21/44/mask-1409287_960_720.jpg",
                MinHealth = 16,
                MaxHealth = 31,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Arcane-Touched Spriggan",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/06/21/08/fantasy-2824654_960_720.jpg",
                MinHealth = 21,
                MaxHealth = 36,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Assassin",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/03/31/21/41/assassin-1296592_960_720.png",
                MinHealth = 1,
                MaxHealth = 11,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Balrog",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/1/14/Durin%27s_Bane.jpg",
                MinHealth = 7,
                MaxHealth = 22,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bandit",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/13/10/24/burglar-157142_960_720.png",
                MinHealth = 0,
                MaxHealth = 14,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Barred Owl",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/11/05/09/03/owl-517497_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 15,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Basilisk",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/70/Wenceslas_Hollar_-_The_basilisk_and_the_weasel.jpg",
                MinHealth = 47,
                MaxHealth = 62,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bat",
                ImageUrl = "https://live.staticflickr.com/3741/9187712502_bbecd929e1_b.jpg",
                MinHealth = 0,
                MaxHealth = 11,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bear",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/03/27/18/10/bear-1283347_960_720.jpg",
                MinHealth = 9,
                MaxHealth = 24,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Black Wyrm",
                ImageUrl = "https://kagesatsuki.files.wordpress.com/2017/05/170531_02.jpg",
                MinHealth = 39,
                MaxHealth = 54,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Blood Wyrm",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/08/20/16/43/dragon-2662375_960_720.jpg",
                MinHealth = 49,
                MaxHealth = 64,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bloodrage Drake",
                ImageUrl = "https://i.imgur.com/dxEjIsT.jpg",
                MinHealth = 55,
                MaxHealth = 70,
                MinAttackPower = 8,
                MaxAttackPower = 15
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bloodtooth Wolf",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/02/13/11/57/wolf-635063_960_720.jpg",
                MinHealth = 2,
                MaxHealth = 17,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bog Beast",
                ImageUrl = "https://www.goodfreephotos.com/cache/art-and-illustrations/hand-coming-up-from-the-swamp_800.jpg",
                MinHealth = 14,
                MaxHealth = 29,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bronze Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/13/07/57/dragon-1976596_960_720.jpg",
                MinHealth = 40,
                MaxHealth = 55,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bronze Guardian",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/10/27/14/17/demon-201422_960_720.jpg",
                MinHealth = 20,
                MaxHealth = 35,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Bronze Wyrm",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/7b/Dragon-Linda_BlackWin24_Jansson.jpg",
                MinHealth = 53,
                MaxHealth = 68,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Brown Eagle",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/12/01/19/35/portrait-1072696_960_720.jpg",
                MinHealth = 1,
                MaxHealth = 16,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Carved Totem",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/06/01/10/53/face-793186_960_720.jpg",
                MinHealth = 24,
                MaxHealth = 39,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Cave Lion",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/25/16/54/african-lion-2888519_960_720.jpg",
                MinHealth = 16,
                MaxHealth = 31,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Cave Rat",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/07/22/21/29/color-rat-399706_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 9,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Celestial Unicorn",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/02/03/13/54/unicorn-2035174_960_720.png",
                MinHealth = 20,
                MaxHealth = 35,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Celestial Wyrm",
                ImageUrl = "https://i.imgur.com/BbBmEOF.jpg",
                MinHealth = 75,
                MaxHealth = 90,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Centaur Archwizard",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/02/17/09/23/fantasy-3159483_960_720.jpg",
                MinHealth = 9,
                MaxHealth = 24,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Centaur Mage",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/13/12/16/centaur-159538_960_720.png",
                MinHealth = 31,
                MaxHealth = 46,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Centaur Spirit-talker",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/13/12/16/centaur-159538_960_720.png",
                MinHealth = 6,
                MaxHealth = 21,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Charmed Wyrm Statue",
                ImageUrl = "https://s0.geograph.org.uk/geophotos/04/91/10/4911075_244f2d29.jpg",
                MinHealth = 27,
                MaxHealth = 42,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Corrupted Dryad",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/05/14/19/45/dryad-1392438_960_720.jpg",
                MinHealth = 25,
                MaxHealth = 40,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Corrupted Phoenix",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/06/06/21/28/phoenix-1440452_960_720.jpg",
                MinHealth = 22,
                MaxHealth = 37,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Corrupted Unicorn",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/04/02/04/53/colorful-1302152_960_720.png",
                MinHealth = 27,
                MaxHealth = 42,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Crystal Drake",
                ImageUrl = "https://i.imgur.com/4HWOp15.jpg",
                MinHealth = 62,
                MaxHealth = 77,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Cultist Lord",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/09/21/08/14/fantasy-2771073_960_720.jpg",
                MinHealth = 16,
                MaxHealth = 31,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Cursed Tentacle",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/08/03/10/15/tail-1566250_960_720.jpg",
                MinHealth = 34,
                MaxHealth = 49,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Dappled Centaur",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/13/12/16/centaur-159538_960_720.png",
                MinHealth = 6,
                MaxHealth = 21,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Deer Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/03/16/21/24/fantasy-3232570_960_720.jpg",
                MinHealth = 9,
                MaxHealth = 24,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Doomsayer Cultist",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/09/14/12/32/forest-1669338_960_720.jpg",
                MinHealth = 8,
                MaxHealth = 23,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Dryad Spellcaster",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/05/14/19/45/dryad-1392438_960_720.jpg",
                MinHealth = 5,
                MaxHealth = 20,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Eagle",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/19/13/42/eagle-1753002_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 10,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Earth Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/08/13/23/08/dragon-2638881_960_720.jpg",
                MinHealth = 65,
                MaxHealth = 80,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Earth Elemental",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/09/08/20/54/elephant-4461911_960_720.jpg",
                MinHealth = 33,
                MaxHealth = 48,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Elder Dragon",
                ImageUrl = "https://vignette.wikia.nocookie.net/monster/images/6/6e/DragonRed.jpg",
                MinHealth = 48,
                MaxHealth = 63,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Elder Forest Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/03/19/11/31/horror-2156302_960_720.jpg",
                MinHealth = 24,
                MaxHealth = 39,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Elder Treant",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/07/22/12/05/green-man-3554439_960_720.jpg",
                MinHealth = 14,
                MaxHealth = 29,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Emerald Drake",
                ImageUrl = "https://i.imgur.com/cZZ6RJh.png",
                MinHealth = 46,
                MaxHealth = 61,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Emerald Oracle",
                ImageUrl = "https://i.imgur.com/So3u7Ni.png",
                MinHealth = 37,
                MaxHealth = 52,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Emerald Wyrm",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/08/05/00/10/dragon-1571287_960_720.png",
                MinHealth = 41,
                MaxHealth = 56,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Enraged Drake",
                ImageUrl = "https://i.imgur.com/rCHhnqt.jpg",
                MinHealth = 58,
                MaxHealth = 73,
                MinAttackPower = 8,
                MaxAttackPower = 15
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Enraged Oracle",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/03/10/23/13/oracle-girl-2133976_960_720.jpg",
                MinHealth = 38,
                MaxHealth = 53,
                MinAttackPower = 5,
                MaxAttackPower = 12
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ethereal Bird Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/05/17/18/49/pentecost-3409407_960_720.jpg",
                MinHealth = 46,
                MaxHealth = 61,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ethereal Cat Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/03/09/07/45/lion-283639_960_720.jpg",
                MinHealth = 46,
                MaxHealth = 61,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ethereal Wolf Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/07/31/17/38/wolf-2559374_960_720.jpg",
                MinHealth = 46,
                MaxHealth = 61,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Faerie",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/02/26/23/41/fairy-2101714_960_720.png",
                MinHealth = 24,
                MaxHealth = 39,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fel-Corrupted Satyr",
                ImageUrl = "https://banner2.kisspng.com/20180205/vjq/kisspng-demon-idea-demon-png-transparent-5a790f727e5a00.3066432715178832505175.jpg",
                MinHealth = 22,
                MaxHealth = 37,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fire Dragon",
                ImageUrl = "https://live.staticflickr.com/3156/2806388133_4332cc63b1_z.jpg",
                MinHealth = 55,
                MaxHealth = 70,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fire Elemental",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/04/16/21/32/flame-726268_960_720.jpg",
                MinHealth = 32,
                MaxHealth = 47,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fire Fairy",
                ImageUrl = "https://www.maxpixel.net/static/photo/1x/Fee-Magic-Mystical-Fantasy-Fire-Surreal-Elf-3350952.jpg",
                MinHealth = 23,
                MaxHealth = 38,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fire Hawk",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/13/09/35/eagle-155747_960_720.png",
                MinHealth = 7,
                MaxHealth = 22,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fire Salamander",
                ImageUrl = "https://cdn.pixabay.com/photo/2012/08/18/14/00/fire-salamander-54533_960_720.jpg",
                MinHealth = 21,
                MaxHealth = 36,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fire Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/03/16/21/24/fantasy-3232570_960_720.jpg",
                MinHealth = 12,
                MaxHealth = 27,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fire Wyvern",
                ImageUrl = "https://i.imgur.com/x8RIYvo.jpeg",
                MinHealth = 37,
                MaxHealth = 52,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Flatland Cougar",
                ImageUrl = "https://cdn.pixabay.com/photo/2022/01/27/05/11/cougar-6971086_960_720.jpg",
                MinHealth = 4,
                MaxHealth = 19,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fog Beast",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/13/14/15/fantasy-2847724_960_720.jpg",
                MinHealth = 29,
                MaxHealth = 44,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fog Beast Minion",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/22/18/30/anxiety-2878767_960_720.jpg",
                MinHealth = 29,
                MaxHealth = 44,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Atronach",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/06/19/20/fantasy-2824304_960_720.jpg",
                MinHealth = 13,
                MaxHealth = 28,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Drake",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/06/08/17/23/dragon-3462724_960_720.jpg",
                MinHealth = 64,
                MaxHealth = 79,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Fairie",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/03/16/15/56/woman-1261048_960_720.png",
                MinHealth = 2,
                MaxHealth = 17,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Guardian",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/06/10/22/58/composing-2391033_960_720.jpg",
                MinHealth = 4,
                MaxHealth = 19,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Kirin",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/02/12/14/01/horse-3148414_960_720.png",
                MinHealth = 23,
                MaxHealth = 38,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/12/29/10/44/magician-3047235_960_720.jpg",
                MinHealth = 23,
                MaxHealth = 38,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Sprite",
                ImageUrl = "https://cdn.pixabay.com/photo/2012/02/28/15/47/swirls-18530_960_720.jpg",
                MinHealth = 1,
                MaxHealth = 16,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Troll",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/07/20/00/00/ork-2520855_960_720.jpg",
                MinHealth = 27,
                MaxHealth = 42,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Forest Witch",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/25/00/50/third-eye-2886688_960_720.jpg",
                MinHealth = 35,
                MaxHealth = 50,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Fox Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/04/24/19/42/fuchs-2257543_960_720.jpg",
                MinHealth = 7,
                MaxHealth = 22,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Frost Elemental",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/06/19/15/soap-bubble-1958650_960_720.jpg",
                MinHealth = 33,
                MaxHealth = 48,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Frost Spider",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/12/09/11/16/web-spider-1894655_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 10,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Frost Troll",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/10/13/29/creature-1969074_960_720.png",
                MinHealth = 26,
                MaxHealth = 41,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Giant Eagle",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/08/28/12/21/giant-eagle-2689367_960_720.jpg",
                MinHealth = 1,
                MaxHealth = 16,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Giant Roc",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/02/06/20/08/eagle-2044134_960_720.jpg",
                MinHealth = 13,
                MaxHealth = 28,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Gnoll",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/2/2f/DnD_Gnoll.png",
                MinHealth = 0,
                MaxHealth = 9,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Golden Amphitere",
                ImageUrl = "https://i.imgur.com/k5BWr2K.jpg",
                MinHealth = 69,
                MaxHealth = 84,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Golden Draconid",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/05/18/22/52/dragon-3412281_960_720.png",
                MinHealth = 38,
                MaxHealth = 53,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Golden Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/01/05/01/19/dragon-238931_960_720.jpg",
                MinHealth = 51,
                MaxHealth = 66,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Golden Guardian Wyrm",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/10/19/05/21/sculpture-197772_960_720.jpg",
                MinHealth = 89,
                MaxHealth = 100,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Golden Wyrm",
                ImageUrl = "https://i.imgur.com/zOd2s9R.jpg",
                MinHealth = 44,
                MaxHealth = 59,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Goose",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/04/25/21/30/wild-goose-2260866_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 15,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Guardian Statue",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/04/12/09/13/fantasy-4121663_960_720.jpg",
                MinHealth = 26,
                MaxHealth = 41,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Harpy Matriarch",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/07/10/22/17/harpy-2491774_960_720.jpg",
                MinHealth = 20,
                MaxHealth = 35,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Harpy Windsinger",
                ImageUrl = "https://www.maxpixel.net/static/photo/1x/Bird-Zoo-Harpy-Berlin-2772092.jpg",
                MinHealth = 17,
                MaxHealth = 32,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Harpy Witch",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/01/31/18/44/harpie-618781_960_720.jpg",
                MinHealth = 5,
                MaxHealth = 20,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Horse Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/12/25/16/53/spirit-579807_960_720.jpg",
                MinHealth = 28,
                MaxHealth = 43,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ice Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/08/19/23/27/dragon-4417431_960_720.png",
                MinHealth = 41,
                MaxHealth = 56,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ice Elemental",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/11/07/00/07/fantasy-2925250_960_720.jpg",
                MinHealth = 33,
                MaxHealth = 48,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ice Wyvern",
                ImageUrl = "https://gourmetpapermache.files.wordpress.com/2018/10/1a.jpg",
                MinHealth = 39,
                MaxHealth = 54,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Imp",
                ImageUrl = "https://media-waterdeep.cursecdn.com/avatars/thumbnails/0/361/234/315/636252778560366227.jpeg",
                MinHealth = 0,
                MaxHealth = 7,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Incubus",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/7e/Incubus.jpg",
                MinHealth = 23,
                MaxHealth = 38,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Jade Drake",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/25/22/55/dragon-1770252_960_720.jpg",
                MinHealth = 55,
                MaxHealth = 70,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Jaguar",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/09/07/04/02/jaguar-2723827_960_720.jpg",
                MinHealth = 13,
                MaxHealth = 28,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Kobold",
                ImageUrl = "https://vignette.wikia.nocookie.net/forgottenrealms/images/f/f3/Monster_Manual_5e_-_Kobold_-_p195.jpg",
                MinHealth = 0,
                MaxHealth = 9,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Lava Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2012/11/28/09/46/lava-67574_960_720.jpg",
                MinHealth = 34,
                MaxHealth = 49,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Lesser Felguard",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/09/20/23/44/hell-454463_960_720.jpg",
                MinHealth = 25,
                MaxHealth = 40,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Lightning Dragon",
                ImageUrl = "https://wallpapercave.com/wp/wp1968832.jpg",
                MinHealth = 47,
                MaxHealth = 62,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Lone Wolf",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/01/06/03/38/wolf-3916379_960_720.jpg",
                MinHealth = 1,
                MaxHealth = 11,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Magma Giant",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/07/30/09/00/arc-405098_960_720.jpg",
                MinHealth = 24,
                MaxHealth = 39,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Minotaur",
                ImageUrl = "https://i.imgur.com/k3fQO6t.png",
                MinHealth = 36,
                MaxHealth = 51,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Minotaur Berserker",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/03/17/11/minotaur-1950012_960_720.png",
                MinHealth = 46,
                MaxHealth = 61,
                MinAttackPower = 8,
                MaxAttackPower = 15
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Mountain Troll",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/14/11/21/fantasy-1739989_960_720.jpg",
                MinHealth = 7,
                MaxHealth = 22,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Mouse",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/01/20/54/mouse-1708347_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 6,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Nether Dragon",
                ImageUrl = "https://i.imgur.com/Y3fFqUX.jpg",
                MinHealth = 41,
                MaxHealth = 56,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Night Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/02/28/11/35/dragons-653439_960_720.jpg",
                MinHealth = 51,
                MaxHealth = 66,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Obsidian Drake",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/12/06/08/39/dragon-3001238_960_720.png",
                MinHealth = 42,
                MaxHealth = 57,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Obsidian Vapor Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/11/08/05/13/acid-1807514_960_720.jpg",
                MinHealth = 38,
                MaxHealth = 53,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ogre",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/07/17/15/10/ogre-1524018_960_720.png",
                MinHealth = 3,
                MaxHealth = 18,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ogre Bruiser",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/04/01/10/30/armour-1299911_960_720.png",
                MinHealth = 3,
                MaxHealth = 18,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Orc Peon",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/68/Savage_Orc_by_farmerownia.jpg/220px-Savage_Orc_by_farmerownia.jpg",
                MinHealth = 0,
                MaxHealth = 12,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Orc Shieldbreaker",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/04/17/20/14/troll-3328592_960_720.png",
                MinHealth = 20,
                MaxHealth = 35,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Orc Slavedriver",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/07/23/19/vintage-1722810_960_720.jpg",
                MinHealth = 3,
                MaxHealth = 18,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Owlspider",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/12/05/19/10/speule-3000150_960_720.jpg",
                MinHealth = 11,
                MaxHealth = 26,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Pack of Frost Wolves",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/18/16/08/wolf-2864647_960_720.jpg",
                MinHealth = 40,
                MaxHealth = 55,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Pack of Sabre Cats",
                ImageUrl = "https://www.nps.gov/whsa/learn/nature/images/Saber_Tooth.jpg",
                MinHealth = 36,
                MaxHealth = 51,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Pack of Wolves",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/08/25/20/59/timber-wolves-907680_960_720.jpg",
                MinHealth = 29,
                MaxHealth = 44,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Peacock Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/25/12/53/peacock-2887882_960_720.jpg",
                MinHealth = 28,
                MaxHealth = 43,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Petty Thief",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/13/10/24/burglar-157142_960_720.png",
                MinHealth = 0,
                MaxHealth = 8,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Plagued Deer",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/03/11/16/38/hirsch-3217239_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 15,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Possessed Sword",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/02/10/22/55/sword-3144759_960_720.png",
                MinHealth = 6,
                MaxHealth = 21,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Possessed Tome",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/25/18/18/book-1769625_960_720.png",
                MinHealth = 10,
                MaxHealth = 25,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Primal Forest Elemental",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/03/09/02/12/eyes-4043559_960_720.jpg",
                MinHealth = 62,
                MaxHealth = 77,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Primal Light Elemental",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/01/22/12/19/travel-3098815_960_720.jpg",
                MinHealth = 62,
                MaxHealth = 77,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Primal Shaman",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/03/30/20/06/fantasy-4091953_960_720.jpg",
                MinHealth = 35,
                MaxHealth = 50,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Pterosaur",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/09/10/13/06/pterosaur-2735500_960_720.jpg",
                MinHealth = 32,
                MaxHealth = 47,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Quintessence Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/09/13/21/29/fantasy-2747066_960_720.jpg",
                MinHealth = 95,
                MaxHealth = 100,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Rabbit Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/06/25/19/47/hare-3497805_960_720.jpg",
                MinHealth = 2,
                MaxHealth = 12,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Rabid Boar",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/04/02/10/55/boar-304954_960_720.png",
                MinHealth = 12,
                MaxHealth = 27,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Rabid Cave Bear",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/11/15/18/14/grizzlies-210996_960_720.jpg",
                MinHealth = 17,
                MaxHealth = 32,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ram",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/12/17/15/58/mammal-3024471_960_720.jpg",
                MinHealth = 2,
                MaxHealth = 17,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Rat",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/04/09/10/13/degu-3303715_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 6,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Raven",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/0/00/Bryce_Canyon_National_Park%2C_United_States_%28Unsplash_T5Ye7puWZxo%29.jpg/640px-Bryce_Canyon_National_Park%2C_United_States_%28Unsplash_T5Ye7puWZxo%29.jpg",
                MinHealth = 0,
                MaxHealth = 9,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Reanimated Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/06/14/14/dragon-1957809_960_720.png",
                MinHealth = 53,
                MaxHealth = 68,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Reanimated Skeleton",
                ImageUrl = "https://cdn.pixabay.com/photo/2012/04/12/14/03/skeleton-30160_960_720.png",
                MinHealth = 11,
                MaxHealth = 26,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Reanimated Statue",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/10/27/14/28/face-201442_960_720.jpg",
                MinHealth = 31,
                MaxHealth = 46,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Reanimated Totemic Guardian",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/02/28/03/02/sculpture-3187129_960_720.jpg",
                MinHealth = 27,
                MaxHealth = 42,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Red Dragon",
                ImageUrl = "https://i.imgur.com/KN0FcNT.jpg",
                MinHealth = 44,
                MaxHealth = 59,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Red Squirrel",
                ImageUrl = "https://cdn.pixabay.com/photo/2012/02/28/00/49/squirrel-17854_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 11,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Restless Ghost",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/03/30/01/04/ghosts-2186959_960_720.jpg",
                MinHealth = 18,
                MaxHealth = 33,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Restless Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/05/11/15/03/spirit-2304469_960_720.jpg",
                MinHealth = 27,
                MaxHealth = 42,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Rotting Slime",
                ImageUrl = "https://i.imgur.com/uL6L5Qj.png",
                MinHealth = 1,
                MaxHealth = 16,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Ruby Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/12/30/11/17/dragon-1940366_960_720.png",
                MinHealth = 44,
                MaxHealth = 59,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Sabre Cat",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/05/07/13/49/tiger-2292496_960_720.jpg",
                MinHealth = 13,
                MaxHealth = 28,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Sandskin Wyvern",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/9/9c/Scale-adult-preview.png",
                MinHealth = 50,
                MaxHealth = 65,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Satyr",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/05/22/21/44/mask-1409287_960_720.jpg",
                MinHealth = 12,
                MaxHealth = 27,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Satyr Bard",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/31/21/17/faun-2027325_960_720.png",
                MinHealth = 10,
                MaxHealth = 25,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Satyr Lord",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/02/21/13/11/hells-spirits-644224_960_720.jpg",
                MinHealth = 27,
                MaxHealth = 42,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Satyr Spellcaster",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/05/22/21/44/mask-1409287_960_720.jpg",
                MinHealth = 12,
                MaxHealth = 27,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Sea Dragon",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/03/03/22/27/fantasy-4033001_960_720.jpg",
                MinHealth = 51,
                MaxHealth = 66,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Seagull",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/05/13/14/03/sea-gull-765490_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 11,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Silver Wyrm",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/03/17/04/dragon-1949993_960_720.png",
                MinHealth = 47,
                MaxHealth = 62,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Siren",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/09/25/19/23/mermaid-2786382_960_720.png",
                MinHealth = 1,
                MaxHealth = 11,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Skeleton Berserker",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/23/17/02/fantasy-2881646_960_720.jpg",
                MinHealth = 6,
                MaxHealth = 21,
                MinAttackPower = 4,
                MaxAttackPower = 11
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Skeleton Captain",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/05/24/20/51/skull-782385_960_720.png",
                MinHealth = 32,
                MaxHealth = 47,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Skeleton Troop Leader",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/05/01/20/45/death-head-2276610_960_720.jpg",
                MinHealth = 5,
                MaxHealth = 20,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Skeleton Trooper",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/02/18/19/02/halloween-4005104_960_720.png",
                MinHealth = 23,
                MaxHealth = 38,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Skull Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/02/17/22/25/skull-3161081_960_720.jpg",
                MinHealth = 31,
                MaxHealth = 46,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Slime",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/12/14/54/slime-148995_960_720.png",
                MinHealth = 0,
                MaxHealth = 9,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Snake",
                ImageUrl = "https://cdn.pixabay.com/photo/2014/11/23/21/22/green-tree-python-543243_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 6,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Spectral Kingfisher",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/12/19/18/43/fantasy-3028475_960_720.jpg",
                MinHealth = 21,
                MaxHealth = 36,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Spectral Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/02/08/17/24/butterfly-2049567_960_720.jpg",
                MinHealth = 7,
                MaxHealth = 22,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Spriggan",
                ImageUrl = "https://live.staticflickr.com/2814/34070170805_aa8bb0fb74_b.jpg",
                MinHealth = 20,
                MaxHealth = 35,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Striped Kestrel",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/12/04/10/01/animal-3855236_960_720.jpg",
                MinHealth = 2,
                MaxHealth = 17,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Succubus",
                ImageUrl = "https://dibujando.net/files/fs/p/c/900x1000/2016/297/SuccubusnInprnt_290935.jpg",
                MinHealth = 9,
                MaxHealth = 24,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Swan",
                ImageUrl = "https://cdn.pixabay.com/photo/2019/05/01/09/50/swan-4170400_960_720.jpg",
                MinHealth = 4,
                MaxHealth = 19,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Sylph",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/02/01/10/19/creature-2029421_960_720.png",
                MinHealth = 0,
                MaxHealth = 10,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Sylvan Unicorn",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/01/15/11/08/unicorn-1981219_960_720.jpg",
                MinHealth = 20,
                MaxHealth = 35,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Tame Dove",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/02/23/20/31/dove-1218474_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 11,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Three-Headed Hydra",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/04/24/10/05/monster-2255973_960_720.png",
                MinHealth = 51,
                MaxHealth = 66,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Tiger",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/03/26/20/49/tiger-3264048_960_720.jpg",
                MinHealth = 3,
                MaxHealth = 18,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Totem Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/10/25/18/18/book-1769625_960_720.png",
                MinHealth = 31,
                MaxHealth = 46,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Treant",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/05/19/07/52/fun-773222_960_720.jpg",
                MinHealth = 18,
                MaxHealth = 33,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Tree Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/06/09/21/02/tree-2388245_960_720.jpg",
                MinHealth = 19,
                MaxHealth = 34,
                MinAttackPower = 0,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Trickster Faun",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/12/18/41/faun-153712_960_720.png",
                MinHealth = 6,
                MaxHealth = 21,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Trickster Tree Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/12/13/13/33/log-1091115_960_720.jpg",
                MinHealth = 21,
                MaxHealth = 36,
                MinAttackPower = 0,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Troll Berserker",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/04/17/20/15/troll-3328599_960_720.png",
                MinHealth = 7,
                MaxHealth = 22,
                MinAttackPower = 8,
                MaxAttackPower = 15
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Troll Captain",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/04/18/17/26/say-gestalt-3331025_960_720.png",
                MinHealth = 22,
                MaxHealth = 37,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Troll Guard",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/04/18/20/46/troll-3331579_960_720.png",
                MinHealth = 5,
                MaxHealth = 20,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Troll Slave",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/04/17/20/11/troll-3328570_960_720.png",
                MinHealth = 2,
                MaxHealth = 17,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Vapor Spirit",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/09/23/14/46/yellowstone-953835_960_720.jpg",
                MinHealth = 31,
                MaxHealth = 46,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Viridian Amphithere",
                ImageUrl = "https://i.imgur.com/oTi3K3q.jpg",
                MinHealth = 65,
                MaxHealth = 80,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Voidspawn Demon",
                ImageUrl = "http://pngimg.com/uploads/demon/demon_PNG24.png",
                MinHealth = 20,
                MaxHealth = 35,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Voidwalker Demon",
                ImageUrl = "https://cdn.pixabay.com/photo/2013/07/13/13/49/demon-161607_960_720.png",
                MinHealth = 5,
                MaxHealth = 20,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Weasel",
                ImageUrl = "https://cdn.pixabay.com/photo/2018/04/26/14/50/animals-3352109_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 6,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Wild Dove",
                ImageUrl = "https://cdn.pixabay.com/photo/2016/07/16/20/39/dove-1522656_960_720.jpg",
                MinHealth = 0,
                MaxHealth = 11,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Wild Stallion",
                ImageUrl = "https://cdn.pixabay.com/photo/2017/10/31/07/49/horses-2904536_960_720.jpg",
                MinHealth = 3,
                MaxHealth = 18,
                MinAttackPower = 6,
                MaxAttackPower = 13
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Wizard",
                ImageUrl = "https://cdn.pixabay.com/photo/2015/05/18/16/58/abstract-772508_960_720.jpg",
                MinHealth = 41,
                MaxHealth = 56,
                MinAttackPower = 3,
                MaxAttackPower = 10
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Wood Spider",
                ImageUrl = "https://i.stack.imgur.com/S21a4.jpg",
                MinHealth = 5,
                MaxHealth = 20,
                MinAttackPower = 2,
                MaxAttackPower = 9
            },
            new MobPreset
            {
                Id = ++idIndex,
                Name = "Wormwing Demon",
                ImageUrl = "http://pngimg.com/uploads/demon/demon_PNG20.png",
                MinHealth = 15,
                MaxHealth = 30,
                MinAttackPower = 6,
                MaxAttackPower = 13
            }
        );
    }
}