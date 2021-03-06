﻿using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using ReChoGath.Modes;
using ReChoGath.ReCore;
using System;
using SharpDX;
using ReChoGath.Utils;
using System.Linq;

namespace ReChoGath
{
    class Program
    {
        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Chogath") return;
            
            VersionChecker.Check();
            Loader.Initialize(); // ReCore BETA
            Humanizer.Initialize();
            MenuLoader.Initialize();
            Drawing.OnDraw += OnDraw;
            Game.OnTick += OnTick;
            Game.OnUpdate += OnTick;
            Orbwalker.OnUnkillableMinion += LastHit.OnUnkillableMinion;
            Drawing.OnEndScene += OnEndScene;

            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Orbwalker.OnAttack += Orbwalker_OnAttack;

            Chat.Print("<font color='#FFFFFF'>ReChoGath v." + VersionChecker.AssVersion + " has been loaded.</font>");
        }

        public static int LastAttack = Core.GameTickCount;
        private static void Orbwalker_OnAttack(AttackableUnit target, EventArgs args)
        {
            LastAttack = Core.GameTickCount;
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Player.Instance.IsDead || !Config.Drawing.Menu.GetCheckBoxValue("Config.Drawing.Indicator"))
                return;

            Indicator.Execute();
        }

        public static void OnTick(EventArgs args)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling()) 
                return;

            if (Core.GameTickCount - LastAttack > 5000) Other.SetSpikes(false);

            PermaActive.Execute();
            var flags = Orbwalker.ActiveModesFlags;
            #region Flags checker
            if (flags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                try
                {
                    Combo.Execute();
                }
                catch (Exception e) 
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            if (flags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                try
                {
                    Harass.Execute();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            if (flags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                try
                {
                    LastHit.Execute();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            if (flags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                try
                {
                    LaneClear.Execute();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            if (flags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                try
                {
                    JungleClear.Execute();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught.", e);
                }
            }
            if (Config.Combo.Menu.GetKeyBindValue("Config.Combo.R.FlashR"))
            {
                if (SpellManager.R.IsReady() && SpellManager.Flash.IsReady())
                {
                    foreach (var e in EntityManager.Heroes.Enemies.Where(h => h.IsValid && h.IsAlive() && !h.IsInvulnerable))
                    {
                        if (!e.HasSpellshield() && e.IsInRange(Player.Instance, SpellManager.R.Range + 200))
                        {
                            Other.FlashR(e);
                            break;
                        }
                    }
                }
            }
            #endregion
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!Config.Misc.Menu.GetCheckBoxValue("Config.Misc.Another.Gapcloser") || !sender.IsValidTarget(SpellManager.Q.Range)) return;

            if (SpellManager.W.IsReady() && sender.IsInRange(Player.Instance, SpellManager.W.Range))
            {
                Core.DelayAction(() => SpellManager.W.Cast(sender), Config.Misc.Menu.GetSliderValue("Config.Misc.Another.Delay"));
                return;
            }

            if (SpellManager.Q.IsReady() && sender.IsInRange(Player.Instance, SpellManager.Q.Range))
            {
                var position = SpellManager.Q.GetPrediction(sender);
                Core.DelayAction(() => SpellManager.Q.Cast(position.CastPosition), Config.Misc.Menu.GetSliderValue("Config.Misc.Another.Delay"));
                return;
            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!Config.Misc.Menu.GetCheckBoxValue("Config.Misc.Another.Interrupter") || !sender.IsValidTarget(SpellManager.Q.Range)) return;

            if (SpellManager.W.IsReady() && sender.IsInRange(Player.Instance, SpellManager.W.Range))
            {
                Core.DelayAction(() => SpellManager.W.Cast(sender), Config.Misc.Menu.GetSliderValue("Config.Misc.Another.Delay"));
                return;
            }

            if (SpellManager.Q.IsReady() && sender.IsInRange(Player.Instance, SpellManager.Q.Range))
            {
                Core.DelayAction(() => SpellManager.Q.Cast(sender), Config.Misc.Menu.GetSliderValue("Config.Misc.Another.Delay"));
                return;
            }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead)
                return;

            foreach (var spell in SpellManager.AllSpells)
            {
                switch (spell.Slot)
                {
                    case SpellSlot.Q:
                        if (!Config.Drawing.Menu.GetCheckBoxValue("Config.Drawing.Q")) continue;
                        break;
                    case SpellSlot.W:
                        if (!Config.Drawing.Menu.GetCheckBoxValue("Config.Drawing.W")) continue;
                        break;
                    case SpellSlot.R:
                        if (!Config.Drawing.Menu.GetCheckBoxValue("Config.Drawing.R")) continue;
                        break;
                }
                Circle.Draw(spell.GetColor(), spell.Range, Player.Instance);
            }
        }
    }
}
