﻿using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using System.Collections.Generic;

namespace ReAhri
{
    public static class SpellManager
    {
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Active W { get; private set; }
        public static Spell.Skillshot E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        public static List<Spell.SpellBase> AllSpells { get; private set; }
        public static Dictionary<SpellSlot, Color> ColorTranslation { get; private set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1500, 125)
            {
                AllowedCollisionCount = int.MaxValue
            };
            W = new Spell.Active(SpellSlot.W, 580);
            E = new Spell.Skillshot(SpellSlot.E, 970, SkillShotType.Linear, 250, 1500, 100)
            {
                AllowedCollisionCount = 0
            };
            R = new Spell.Skillshot(SpellSlot.R, 400, SkillShotType.Linear);

            AllSpells = new List<Spell.SpellBase>(new Spell.SpellBase[] { Q, W, E, R });
            ColorTranslation = new Dictionary<SpellSlot, Color>
            {
                { SpellSlot.Q, Color.LimeGreen.ToArgb(150) },
                { SpellSlot.W, Color.CornflowerBlue.ToArgb(150) },
                { SpellSlot.E, Color.YellowGreen.ToArgb(150) },
                { SpellSlot.R, Color.OrangeRed.ToArgb(150) }
            };
        }

        public static void Initialize()
        {
        }

        public static Color ToArgb(this Color color, byte a) // by Hellsing 
        {
            return new ColorBGRA(color.R, color.G, color.B, a);
        }

        public static Color GetColor(this Spell.SpellBase spell) // by Hellsing 
        {
            return ColorTranslation.ContainsKey(spell.Slot) ? ColorTranslation[spell.Slot] : Color.Wheat;
        }
    }
}