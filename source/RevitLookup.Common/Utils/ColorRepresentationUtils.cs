// Copyright (c) Lookup Foundation and Contributors
// 
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
// 
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Globalization;
using Color = System.Drawing.Color;

namespace RevitLookup.Common.Utils;

/// <summary>
///     Helper class to easier work with color representation
/// </summary>
/// <remarks>
///     Implementation: https://github.com/microsoft/PowerToys/blob/main/src/modules/colorPicker/ColorPickerUI/Helpers/ColorRepresentationHelper.cs
/// </remarks>
public static class ColorRepresentationUtils
{
    private static readonly Dictionary<string, string> KnownColorNames = new()
    {
        {"000000", "Black"},
        {"000080", "Navy blue"},
        {"0000FF", "Blue"},
        {"0018A8", "Blue (Pantone)"},
        {"002E63", "Cool Black"},
        {"003153", "Prussian blue"},
        {"003366", "Midnight Blue"},
        {"003399", "Powder blue"},
        {"003A6C", "Ateneo blue"},
        {"004225", "British racing green"},
        {"0047AB", "Cobalt"},
        {"0048BA", "Absolute zero"},
        {"00563F", "Castleton green"},
        {"006A4E", "Bottle green"},
        {"006B3C", "Cadmium green"},
        {"007BA7", "Cerulean"},
        {"007DFF", "Gradus Blue"},
        {"007FFF", "Azure"},
        {"008080", "Teal"},
        {"0088DC", "Blue cola"},
        {"0093AF", "Blue (Munsell)"},
        {"0095B6", "Bondi Blue"},
        {"00A86B", "Jade"},
        {"00B9FB", "Blue Bolt"},
        {"00BFFF", "Deep sky blue"},
        {"00C4B0", "Amazonite"},
        {"00CC99", "Caribbean green"},
        {"00CCCC", "Robin egg blue"},
        {"00FF00", "Green"},
        {"00FF7F", "Spring Green"},
        {"00FFFF", "Aqua"},
        {"013220", "Dark green"},
        {"01796F", "Pine Green"},
        {"03C03C", "Dark pastel green"},
        {"064E40", "Blue-green (color wheel)"},
        {"082567", "Sapphire"},
        {"08457E", "Dark cerulean"},
        {"08E8DE", "Bright turquoise"},
        {"0A1195", "Cadmium blue"},
        {"0BDA51", "Malachite"},
        {"0D98BA", "Blue-green"},
        {"116062", "Dark turquoise"},
        {"120A8F", "Ultramarine"},
        {"126180", "Blue sapphire"},
        {"141414", "Chinese black"},
        {"1560BD", "Denim"},
        {"177245", "Dark spring green"},
        {"1974D2", "Bright navy blue"},
        {"1A4780", "Black Sea"},
        {"1B1811", "Black chocolate"},
        {"1B4D3E", "Brunswick green"},
        {"1C6B72", "Moray"},
        {"1DACD6", "Bright cerulean"},
        {"1E90FF", "Dodger blue"},
        {"1F4037", "Red Sea"},
        {"1F75FE", "Blue (Crayola)"},
        {"21ABCD", "Ball blue"},
        {"232B2B", "Charleston green"},
        {"246BCE", "Celtic Blue"},
        {"24A0ED", "Button Blue"},
        {"253529", "Black leather jacket"},
        {"2A52BE", "Cerulean blue"},
        {"2A8FBD", "Christmas blue"},
        {"2E2D88", "Cosmic Cobalt"},
        {"2E5894", "B'dazzled blue"},
        {"2E8B57", "Sea Green"},
        {"2F4F4F", "Dark slate gray"},
        {"30D5C8", "Turquoise"},
        {"310062", "Dark Indigo"},
        {"318CE7", "Bleu de France"},
        {"333399", "Blue (pigment)"},
        {"3399FF", "Brilliant azure"},
        {"34B334", "American green"},
        {"34C924", "Vert-de-pomme"},
        {"36454F", "Charcoal"},
        {"365194", "Chinese blue"},
        {"391802", "American bronze"},
        {"3A75C4", "Klein Blue"},
        {"3B2F2F", "Black coffee"},
        {"3B3B6D", "American blue"},
        {"3B3C36", "Black olive"},
        {"3B444B", "Arsenic"},
        {"3B7A57", "Amazon"},
        {"3C1421", "Chocolate Kisses"},
        {"3C3024", "Cola"},
        {"3C8D0D", "Christmas green"},
        {"3CAA3C", "Toad in love"},
        {"3D0C02", "Black bean"},
        {"3D2B1F", "Bistre"},
        {"3F000F", "Chocolate Brown"},
        {"40826D", "Viridian"},
        {"4169E1", "Royal Blue"},
        {"423189", "Dark violet"},
        {"442D25", "Coffee"},
        {"45161C", "Fulvous"},
        {"464451", "Anthracite"},
        {"465945", "Gray-asparagus"},
        {"4682B4", "Steel blue"},
        {"480607", "Bulgarian rose"},
        {"4A2C2A", "Brown Coffee"},
        {"4AFF00", "Chlorophyll green"},
        {"4B0082", "Indigo"},
        {"4B3621", "Caf? noir"},
        {"4B5320", "Army green"},
        {"4C2F27", "Acajou"},
        {"4C5866", "Marengo"},
        {"4D1A7F", "Blue-violet (color wheel)"},
        {"4E1609", "Flea belly"},
        {"4F7942", "Fern green"},
        {"4F86F7", "Blueberry"},
        {"5072A7", "Blue yonder"},
        {"50C878", "Emerald"},
        {"54626F", "Black Coral"},
        {"551B8C", "American violet"},
        {"553592", "Blue-magenta violet"},
        {"556832", "Dark Olive"},
        {"560319", "Dark Scarlet"},
        {"568203", "Avocado"},
        {"56A0D3", "Carolina blue"},
        {"58111A", "Chocolate Cosmos"},
        {"592720", "Caput mortuum"},
        {"5D2B2C", "Christmas brown"},
        {"5D8AA8", "Air Force Navy(RAF)"},
        {"5DA130", "Grass"},
        {"5DADEC", "Blue Jeans"},
        {"5F1933", "Brown Chocolate"},
        {"630F0F", "Blood (Organ)"},
        {"63775B", "Axolotl"},
        {"6495ED", "Cornflower blue"},
        {"654321", "Dark brown"},
        {"660000", "Blood red"},
        {"660066", "Plum"},
        {"660099", "Purple"},
        {"6600FF", "Persian blue"},
        {"663398", "Christmas purple"},
        {"665D1E", "Antique bronze"},
        {"6699CC", "Blue-gray"},
        {"66B447", "Apple"},
        {"66FF00", "Bright green"},
        {"6B4423", "Brown-nose"},
        {"6B8E23", "Olive Drab"},
        {"6E7F80", "AuroMetalSaurus"},
        {"702963", "Byzantium"},
        {"703642", "Catawba"},
        {"704214", "Sepia"},
        {"708090", "Slate gray"},
        {"720B98", "Chinese purple"},
        {"72A0C1", "Air superiority blue"},
        {"734A12", "Raw umber"},
        {"735184", "Seroburomalinovyj"},
        {"7366BD", "Blue-violet (Crayola)"},
        {"737000", "Bronze Yellow"},
        {"755A57", "Russet"},
        {"77DD77", "Pastel green"},
        {"78866B", "Camouflage green"},
        {"79443B", "Bole"},
        {"79A0C1", "Bluish"},
        {"7B3F00", "Cinnamon"},
        {"7B917B", "Fainted frog"},
        {"7BA05B", "Asparagus"},
        {"7BB661", "Bud green"},
        {"7C0A02", "Barn red"},
        {"7CB9E8", "Aero"},
        {"7DF9FF", "Electric"},
        {"7F1734", "Claret"},
        {"7F3E98", "Cadmium violet"},
        {"7FC7FF", "Sky"},
        {"7FFF00", "Chartreuse"},
        {"7FFFD4", "Aquamarine"},
        {"800000", "Maroon"},
        {"800020", "Burgundy"},
        {"804040", "American brown"},
        {"808000", "Olive"},
        {"808080", "Gray"},
        {"81613C", "Coyote brown"},
        {"834D18", "Byron"},
        {"841B2D", "Antique ruby"},
        {"848482", "Battleship grey"},
        {"84DE02", "Alien Armpit"},
        {"856088", "Chinese violet"},
        {"87413F", "Brandy"},
        {"87A96B", "Asparagus"},
        {"884535", "Brick"},
        {"893F45", "Cordovan"},
        {"89CFF0", "Baby blue"},
        {"8A0303", "Blood"},
        {"8A2BE2", "Blue-violet"},
        {"8A3324", "Burnt umber"},
        {"8B00FF", "Violet"},
        {"8C92AC", "Cool grey"},
        {"8DB600", "Apple Green"},
        {"8F5973", "Blackberry"},
        {"8F9779", "Artichoke"},
        {"900020", "Burgundy"},
        {"904D30", "Terracotta"},
        {"911E42", "Cherry"},
        {"915C83", "Antique fuchsia"},
        {"918151", "Dark tan"},
        {"92000A", "Sangria"},
        {"954535", "Chestnut"},
        {"960018", "Carmine"},
        {"964B00", "Brown"},
        {"965A3E", "Coconut"},
        {"967117", "Bistre brown"},
        {"986960", "Dark chestnut"},
        {"987654", "Pale brown"},
        {"98777B", "Bazaar"},
        {"98817B", "Cinereous"},
        {"98FF98", "Mint Green"},
        {"990066", "Eggplant"},
        {"991199", "Violet-eggplant"},
        {"993366", "Mauve"},
        {"996666", "Copper rose"},
        {"9966CC", "Amethyst"},
        {"997A8D", "Mountbatten pink"},
        {"99958C", "Quartz"},
        {"9B2D30", "Wine red"},
        {"9C2542", "Big dip o’ruby"},
        {"9DB1CC", "Niagara"},
        {"9F2B68", "Amaranth deep purple"},
        {"9F8170", "Beaver"},
        {"9FA91F", "Citron"},
        {"A08040", "Chamois"},
        {"A17A74", "Burnished Brown"},
        {"A1CAF1", "Baby blue eyes"},
        {"A25F2A", "Camelopardalis"},
        {"A2A2D0", "Blue Bell"},
        {"A41313", "Blood (Animal)"},
        {"A4C639", "Android green"},
        {"A5260A", "Bismarck-furious"},
        {"A52A2A", "Auburn"},
        {"A57164", "Blast-off bronze"},
        {"A67B5B", "Caf? au lait"},
        {"A8516E", "China rose"},
        {"AA381E", "Chinese red"},
        {"AB274F", "Amaranth purple"},
        {"AB381F", "Chinese brown"},
        {"ABCDEF", "Pale cornflower blue"},
        {"ACB78E", "Swamp green"},
        {"ACE1AF", "Celadon"},
        {"ACE5EE", "Blue Lagoon"},
        {"AD6F69", "Copper penny"},
        {"ADDFAD", "Moss green"},
        {"ADFF2F", "Green-yellow"},
        {"AF002A", "Alabama crimson"},
        {"AF4035", "Pale carmine"},
        {"AF6E4D", "Brown Sugar"},
        {"AFEEEE", "Pale Blue"},
        {"B01B2E", "Christmas red"},
        {"B08D57", "Bronze (Metallic)"},
        {"B0BF1A", "Acid green"},
        {"B284BE", "African violet"},
        {"B2BEB5", "Ash gray"},
        {"B31B1B", "Carnelian"},
        {"B32134", "American red"},
        {"B5A642", "Brass"},
        {"B60C26", "Cadmium Purple"},
        {"B7410E", "Rust"},
        {"B87333", "Copper"},
        {"B8860B", "Dark goldenrod"},
        {"BADBAD", "Dark Tea Green"},
        {"BBBBBB", "Light Grey"},
        {"BCD4E6", "Beau blue"},
        {"BD33A4", "Byzantine"},
        {"BDB76B", "Dark Khaki"},
        {"BEF574", "Pistachio"},
        {"BF4F51", "Bittersweet shimmer"},
        {"BF94E4", "Bright lavender"},
        {"BFAFB2", "Black Shadows"},
        {"BFFF00", "Bitter lime"},
        {"C0C0C0", "Silver"},
        {"C19A6B", "Camel"},
        {"C32148", "Bright maroon"},
        {"C39953", "Aztec Gold"},
        {"C3B091", "Khaki"},
        {"C41E3A", "Cardinal"},
        {"C46210", "Alloy orange"},
        {"C4D8E2", "Columbia Blue"},
        {"C71585", "Red-violet"},
        {"C7D0CC", "Gris de perle"},
        {"C7FCEC", "Pang"},
        {"C8A2C8", "Lilac"},
        {"C95A49", "Cedar Chest"},
        {"C9A0DC", "Wisteria"},
        {"C9FFE5", "Aero blue"},
        {"CAA906", "Christmas gold"},
        {"CADABA", "Gray-Tea Green"},
        {"CAE00D", "Bitter lemon"},
        {"CB4154", "Brick red"},
        {"CC0000", "Boston University Red"},
        {"CC5500", "Burnt orange"},
        {"CC7722", "Ochre"},
        {"CC8899", "Puce"},
        {"CC9900", "Chinese gold"},
        {"CC9966", "Brown Yellow"},
        {"CCCCCC", "Chinese silver"},
        {"CCCCFF", "Periwinkle"},
        {"CCFF00", "Lime"},
        {"CD00CD", "Bright violet"},
        {"CD5700", "Tenne"},
        {"CD5B45", "Dark coral"},
        {"CD5C5C", "Chestnut"},
        {"CD607E", "Cinnamon Satin"},
        {"CD7F32", "Bronze"},
        {"CD8032", "Chinese bronze"},
        {"CD853F", "Light brown"},
        {"CD9575", "Antique brass"},
        {"CFB53B", "Old Gold"},
        {"CFCFCF", "American silver"},
        {"D0DB61", "Chinese green"},
        {"D0F0C0", "Tea Green"},
        {"D0FF14", "Arctic lime"},
        {"D1001C", "Blood orange"},
        {"D19FE8", "Bright ube"},
        {"D1E231", "Pear"},
        {"D2691E", "Chocolate"},
        {"D2B48C", "Tan"},
        {"D3212D", "Amaranth red"},
        {"D3AF37", "American gold"},
        {"D53E07", "Titian"},
        {"D5713F", "Vanilla"},
        {"D5D5D5", "Abdel Kerim's beard"},
        {"D77D31", "Reddish-brown"},
        {"D891EF", "Bright lilac"},
        {"D8A903", "Dark pear"},
        {"D8BFD8", "Thistle"},
        {"DA70D6", "Orchid"},
        {"DAA520", "Goldenrod"},
        {"DABDAB", "Pale Sandy Brown"},
        {"DAD871", "Vert-de-pеche"},
        {"DB7093", "Pale red-violet"},
        {"DBE9F4", "Azureish white"},
        {"DC143C", "Crimson"},
        {"DDADAF", "Pale chestnut"},
        {"DDE26A", "Booger Buster"},
        {"DE3163", "Cerise"},
        {"DE5D83", "Blush"},
        {"DE6FA1", "China pink"},
        {"DF73FF", "Heliotrope"},
        {"E0218A", "Barbie pink"},
        {"E1DFE0", "Christmas silver"},
        {"E28B00", "Siena"},
        {"E2E5DE", "Chinese white"},
        {"E30022", "Cadmium red"},
        {"E32636", "Alizarin crimson"},
        {"E34234", "Cinnabar"},
        {"E3DAC9", "Bone"},
        {"E4717A", "Candy pink"},
        {"E49B0F", "Gamboge"},
        {"E4D00A", "Citrine"},
        {"E52B50", "Amaranth"},
        {"E6E6FA", "Lavender"},
        {"E75480", "Dark pink"},
        {"E7FEFF", "Bubbles"},
        {"E88E5A", "Big Foot Feet"},
        {"E97451", "Burnt sienna"},
        {"E9967A", "Dark salmon"},
        {"E9D66B", "Arylide yellow"},
        {"EA8DF7", "Violaceous"},
        {"EB4C42", "Carmine pink"},
        {"EBC2AF", "Zinnwaldite"},
        {"EBECF0", "Bright gray"},
        {"ED872D", "Cadmium orange"},
        {"ED9121", "Carrot"},
        {"EEDC82", "Flax"},
        {"EEE0B1", "Cookies and cream"},
        {"EEE6A3", "Perhydor"},
        {"EFAF8C", "Saumon"},
        {"EFBBCC", "Cameo pink"},
        {"EFDECD", "Almond"},
        {"F0DC82", "Buff"},
        {"F0F8FF", "Alice blue"},
        {"F19CBB", "Amaranth pink"},
        {"F1DDCF", "Champagne pink"},
        {"F28E1C", "Beer"},
        {"F2B400", "American yellow"},
        {"F2E8C9", "Light cream"},
        {"F2F0E6", "Alabaster"},
        {"F2F3F4", "Anti-flash white"},
        {"F37042", "Chinese orange"},
        {"F4A460", "Sandy brown"},
        {"F4BBFF", "Brilliant lavender"},
        {"F4C2C2", "Baby pink"},
        {"F4C430", "Saffron"},
        {"F5DEB3", "Wheat"},
        {"F5F5DC", "Beige"},
        {"F7E7CE", "Champagne"},
        {"F7F21A", "Child's surprise"},
        {"F88379", "Congo pink"},
        {"F984E5", "Pale magenta"},
        {"FA6E79", "Begonia"},
        {"FADADD", "Pale pink"},
        {"FADFAD", "Peach-yellow"},
        {"FAE7B5", "Banana Mania"},
        {"FAEBD7", "Antique white"},
        {"FAEEDD", "Scared nymph"},
        {"FAF0E6", "Linen"},
        {"FB607F", "Brink pink"},
        {"FBCCE7", "Classic rose"},
        {"FBCEB1", "Apricot"},
        {"FBEC5D", "Corn"},
        {"FC0FC0", "Hot pink"},
        {"FD7C6E", "Coral Reef"},
        {"FDE910", "Lemon"},
        {"FDEE00", "Aureolin"},
        {"FE6F5E", "Bittersweet"},
        {"FEF200", "Christmas yellow"},
        {"FEFEFA", "Baby powder"},
        {"FF0000", "Red"},
        {"FF0038", "Carmine red"},
        {"FF007F", "Bright pink"},
        {"FF00FF", "Magenta (Fuchsia)"},
        {"FF033E", "American rose"},
        {"FF0800", "Candy apple red"},
        {"FF2052", "Awesome"},
        {"FF2400", "Scarlet"},
        {"FF47CA", "Shocked star"},
        {"FF4D00", "Vermilion"},
        {"FF4F00", "Safety orange"},
        {"FF55A3", "Brilliant rose"},
        {"FF6600", "Christmas orange"},
        {"FF7518", "Pumpkin"},
        {"FF7E00", "Amber (SAE/ECE)"},
        {"FF7F50", "Coral"},
        {"FF8B00", "American orange"},
        {"FF8C69", "Salmon"},
        {"FF91AF", "Baker-Miller pink"},
        {"FF9218", "Jaco"},
        {"FF9900", "Blaze Orange"},
        {"FF9966", "Pink-orange"},
        {"FFA500", "Orange"},
        {"FFA600", "Cheese"},
        {"FFA6C9", "Carnation pink"},
        {"FFA812", "Dark tangerine"},
        {"FFAA1D", "Bright Yellow (Crayola)"},
        {"FFBA00", "Selective yellow"},
        {"FFBCD9", "Cotton candy"},
        {"FFBF00", "Amber"},
        {"FFC0CB", "Pink"},
        {"FFC1CC", "Bubble gum"},
        {"FFCC00", "Tangerine"},
        {"FFCC99", "Peach-orange"},
        {"FFCCCB", "Christmas pink"},
        {"FFD1DC", "Pastel pink"},
        {"FFD59A", "Caramel"},
        {"FFD700", "Gold"},
        {"FFD800", "School bus yellow"},
        {"FFDAB9", "Dark Peach"},
        {"FFDB58", "Mustard"},
        {"FFDEAD", "Navajo white"},
        {"FFE135", "Banana yellow"},
        {"FFE4B2", "Yellow Pink"},
        {"FFE4C4", "Bisque"},
        {"FFE5B4", "Peach"},
        {"FFEBCD", "Blanched almond"},
        {"FFEF00", "Canary yellow"},
        {"FFEFD5", "Papaya whip"},
        {"FFF0F5", "Lavender Blush"},
        {"FFF5EE", "Seashell"},
        {"FFF600", "Cadmium yellow"},
        {"FFF8DC", "Cornsilk"},
        {"FFF8E7", "Cosmic latte"},
        {"FFFACD", "Lemon Cream"},
        {"FFFDD0", "Cream"},
        {"FFFDDF", "Ivory"},
        {"FFFF00", "Yellow"},
        {"FFFF99", "Canary"},
        {"FFFFCC", "Conditioner"},
        {"FFFFFF", "White"},
    };

    /// <summary>
    /// Return a <see cref="string"/> representation of a CMYK color
    /// </summary>
    /// <param name="color">The <see cref="System.Windows.Media.Color"/> for the CMYK color presentation</param>
    /// <returns>A <see cref="string"/> representation of a CMYK color</returns>
    public static string ColorToCmyk(Color color)
    {
        var (cyan, magenta, yellow, blackKey) = ColorFormatUtils.ConvertToCmykColor(color);

        cyan = Math.Round(cyan * 100);
        magenta = Math.Round(magenta * 100);
        yellow = Math.Round(yellow * 100);
        blackKey = Math.Round(blackKey * 100);

        return $"{cyan.ToString(CultureInfo.InvariantCulture)}%"
               + $", {magenta.ToString(CultureInfo.InvariantCulture)}%"
               + $", {yellow.ToString(CultureInfo.InvariantCulture)}%"
               + $", {blackKey.ToString(CultureInfo.InvariantCulture)}%";
    }

    /// <summary>
    /// Return a hexadecimal <see cref="string"/> representation of a RGB color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the hexadecimal presentation</param>
    /// <returns>A hexadecimal <see cref="string"/> representation of a RGB color</returns>
    public static string ColorToHex(Color color)
    {
        const string hexFormat = "x2";

        return $"{color.R.ToString(hexFormat, CultureInfo.InvariantCulture)}"
               + $"{color.G.ToString(hexFormat, CultureInfo.InvariantCulture)}"
               + $"{color.B.ToString(hexFormat, CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation of a HSB color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the HSB color presentation</param>
    /// <returns>A <see cref="string"/> representation of a HSB color</returns>
    public static string ColorToHsb(Color color)
    {
        var (hue, saturation, brightness) = ColorFormatUtils.ConvertToHsbColor(color);

        hue = Math.Round(hue);
        saturation = Math.Round(saturation * 100);
        brightness = Math.Round(brightness * 100);

        return $"{hue.ToString(CultureInfo.InvariantCulture)}"
               + $", {saturation.ToString(CultureInfo.InvariantCulture)}%"
               + $", {brightness.ToString(CultureInfo.InvariantCulture)}%";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation float color styling(0.1f, 0.1f, 0.1f)
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to convert</param>
    /// <returns>a string value (0.1f, 0.1f, 0.1f)</returns>
    public static string ColorToFloat(Color color)
    {
        var (red, green, blue) = (color.R / 255d, color.G / 255d, color.B / 255d);
        const int precision = 2;
        const string floatFormat = "0.##";

        return $"{Math.Round(red, precision).ToString(floatFormat, CultureInfo.InvariantCulture)}f"
               + $", {Math.Round(green, precision).ToString(floatFormat, CultureInfo.InvariantCulture)}f"
               + $", {Math.Round(blue, precision).ToString(floatFormat, CultureInfo.InvariantCulture)}f, 1f";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation decimal color value
    /// </summary>
    /// <param name="color">The <see cref="Color"/> to convert</param>
    /// <returns>a string value number</returns>
    public static string ColorToDecimal(Color color)
    {
        return $"{(color.R * 65536) + (color.G * 256) + color.B}";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation of a HSI color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the HSI color presentation</param>
    /// <returns>A <see cref="string"/> representation of a HSI color</returns>
    public static string ColorToHsi(Color color)
    {
        var (hue, saturation, intensity) = ColorFormatUtils.ConvertToHsiColor(color);

        hue = Math.Round(hue);
        saturation = Math.Round(saturation * 100);
        intensity = Math.Round(intensity * 100);

        return $"{hue.ToString(CultureInfo.InvariantCulture)}"
               + $", {saturation.ToString(CultureInfo.InvariantCulture)}%"
               + $", {intensity.ToString(CultureInfo.InvariantCulture)}%";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation of a HSL color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the HSL color presentation</param>
    /// <returns>A <see cref="string"/> representation of a HSL color</returns>
    public static string ColorToHsl(Color color)
    {
        var (hue, saturation, lightness) = ColorFormatUtils.ConvertToHslColor(color);

        hue = Math.Round(hue);
        saturation = Math.Round(saturation * 100);
        lightness = Math.Round(lightness * 100);

        // Using InvariantCulture since this is used for color representation
        return $"{hue.ToString(CultureInfo.InvariantCulture)}"
               + $", {saturation.ToString(CultureInfo.InvariantCulture)}%"
               + $", {lightness.ToString(CultureInfo.InvariantCulture)}%";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation of a HSV color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the HSV color presentation</param>
    /// <returns>A <see cref="string"/> representation of a HSV color</returns>
    public static string ColorToHsv(Color color)
    {
        var (hue, saturation, value) = ColorFormatUtils.ConvertToHsvColor(color);

        hue = Math.Round(hue);
        saturation = Math.Round(saturation * 100);
        value = Math.Round(value * 100);

        // Using InvariantCulture since this is used for color representation
        return $"{hue.ToString(CultureInfo.InvariantCulture)}"
               + $", {saturation.ToString(CultureInfo.InvariantCulture)}%"
               + $", {value.ToString(CultureInfo.InvariantCulture)}%";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation of a HWB color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the HWB color presentation</param>
    /// <returns>A <see cref="string"/> representation of a HWB color</returns>
    public static string ColorToHwb(Color color)
    {
        var (hue, whiteness, blackness) = ColorFormatUtils.ConvertToHwbColor(color);

        hue = Math.Round(hue);
        whiteness = Math.Round(whiteness * 100);
        blackness = Math.Round(blackness * 100);

        return $"{hue.ToString(CultureInfo.InvariantCulture)}"
               + $", {whiteness.ToString(CultureInfo.InvariantCulture)}%"
               + $", {blackness.ToString(CultureInfo.InvariantCulture)}%";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation of a natural color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the natural color presentation</param>
    /// <returns>A <see cref="string"/> representation of a natural color</returns>
    public static string ColorToNCol(Color color)
    {
        var (hue, whiteness, blackness) = ColorFormatUtils.ConvertToNaturalColor(color);

        whiteness = Math.Round(whiteness * 100);
        blackness = Math.Round(blackness * 100);

        return $"{hue}"
               + $", {whiteness.ToString(CultureInfo.InvariantCulture)}%"
               + $", {blackness.ToString(CultureInfo.InvariantCulture)}%";
    }

    /// <summary>
    /// Return a <see cref="string"/> representation of a RGB color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the RGB color presentation</param>
    /// <returns>A <see cref="string"/> representation of a RGB color</returns>
    public static string ColorToRgb(Color color)
        => $"{color.R.ToString(CultureInfo.InvariantCulture)}"
           + $", {color.G.ToString(CultureInfo.InvariantCulture)}"
           + $", {color.B.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Returns a <see cref="string"/> representation of a CIE LAB color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the CIE LAB color presentation</param>
    /// <returns>A <see cref="string"/> representation of a CIE LAB color</returns>
    public static string ColorToCielab(Color color)
    {
        var (lightness, chromaticityA, chromaticityB) = ColorFormatUtils.ConvertToCielabColor(color);
        lightness = Math.Round(lightness, 2);
        chromaticityA = Math.Round(chromaticityA, 2);
        chromaticityB = Math.Round(chromaticityB, 2);

        return $"{lightness.ToString(CultureInfo.InvariantCulture)}" +
               $", {chromaticityA.ToString(CultureInfo.InvariantCulture)}" +
               $", {chromaticityB.ToString(CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Returns a <see cref="string"/> representation of a CIE XYZ color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the CIE XYZ color presentation</param>
    /// <returns>A <see cref="string"/> representation of a CIE XYZ color</returns>
    public static string ColorToCieXyz(Color color)
    {
        var (x, y, z) = ColorFormatUtils.ConvertToCiexyzColor(color);

        x = Math.Round(x * 100, 4);
        y = Math.Round(y * 100, 4);
        z = Math.Round(z * 100, 4);

        return $"{x.ToString(CultureInfo.InvariantCulture)}" +
               $", {y.ToString(CultureInfo.InvariantCulture)}" +
               $", {z.ToString(CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Return a hexadecimal integer <see cref="string"/> representation of a RGB color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for the hexadecimal integer presentation</param>
    /// <returns>A hexadecimal integer <see cref="string"/> representation of a RGB color</returns>
    public static string ColorToHexInteger(Color color)
    {
        const string hexFormat = "X2";

        return "0xFF"
               + $"{color.R.ToString(hexFormat, CultureInfo.InvariantCulture)}"
               + $"{color.G.ToString(hexFormat, CultureInfo.InvariantCulture)}"
               + $"{color.B.ToString(hexFormat, CultureInfo.InvariantCulture)}";
    }

    /// <summary>
    /// Return a name of a RGB color
    /// </summary>
    /// <param name="color">The <see cref="Color"/> for presentation</param>
    /// <returns>Approximate name of a color based on RGB representation</returns>
    public static string GetColorName(Color color)
    {
        var colorName = string.Empty;
        var closestDistance = double.MaxValue;
        foreach (var entry in KnownColorNames)
        {
            var knownColor = ConvertHexStringToColor(entry.Key);
            var distance = CalculateColorDistance(color, knownColor);

            if (distance < closestDistance)
            {
                colorName = entry.Value;
                closestDistance = distance;
            }
        }

        return colorName;
    }

    private static Color ConvertHexStringToColor(string hex)
    {
        var red = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        var green = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        var blue = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

        return Color.FromArgb(red, green, blue);
    }

    private static double CalculateColorDistance(Color color1, Color color2)
    {
        var deltaR = color1.R - color2.R;
        var deltaG = color1.G - color2.G;
        var deltaB = color1.B - color2.B;

        return Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);
    }
}