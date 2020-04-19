using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AtmosLibrary
{
    /// <summary>
    /// Contains implementation of static class Constants and children that contain definitions for all constant values used
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Contains implementation of static class Gases that contains all definitions for gases (oxygen, plasma, tritium, etc.).
        /// </summary>
        public static class Gases
        {
            public static BGas Oxygen = new BGas
                (
                "o2", 
                "Oxygen", 
                20,
                0
                );
            public static BGas Plasma = new BGas
                (
                "plasma",
                "Plasma",
                200,
                0
                );
            public static BGas CarbonDioxide = new BGas
                (
                "co2", 
                "Carbon Dioxide", 
                30,
                0
                );
            public static BGas Nitrogen = new BGas
                (
                "n2", 
                "Nitrogen", 
                20,
                0
                );
            public static BGas NitrousOxide = new BGas
                (
                "n2o", 
                "Nitrous Oxide", 
                40,
                10
                );
            public static BGas WaterVapor = new BGas
                (
                "watervapor", 
                "Water Vapor", 
                40,
                8
                );
            public static BGas HyperNoblium = new BGas
                (
                "hypernob",
                "Hyper-Noblium", 
                2000,
                0
                );
            public static BGas Nitryl = new BGas
                (
                "no2", 
                "Nitryl", 
                20,
                16
                );
            public static BGas Tritium = new BGas
                (
                "tritium", 
                "Tritium", 
                10,
                1
                );
            public static BGas BZ = new BGas
                (
                "bz", 
                "BZ", 
                20,
                8
                );
            public static BGas Stimulum = new BGas
                (
                "stim", 
                "Stimulum", 
                10,
                7
                );
            public static BGas Pluoxium = new BGas
                (
                "pluox", 
                "Pluoxium", 
                80,
                -10
                );
            public static BGas Miasma = new BGas
                (
                "miasma",
                "Miasma",
                20,
                0);

            /// <summary>
            /// All static base gases in a List<BGas>
            /// </summary>
            public static List<BGas> GasList
            {
                get
                {
                    //convoluted reflection/linq stuff..
                    //basically:
                    //Return everything in class Gases with fields Static or Public that is a BGas
                    return typeof(Gases).GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(g => (BGas)g.GetValue(null))
                        .ToList();
                }
            }

            /// <summary>
            /// Function that finds and returns a static BGas with ID id.
            /// </summary>
            /// <param name="id">id of the basegas being found</param>
            /// <returns>BGas with ID id</returns>
            public static BGas FindBGas(string id)
            {
                return GasList.Find(x => x.id == id);
            }
        }
        /// <summary>
        /// Contains implementation of static class GasReactionDelegates that contains all react delegates for <see cref="Reactions"/>
        /// </summary>
        public static class GasReactionDelegates
        {
            public static bool NobliumSuppressionReactionDelegate(GasMixture air_contents)
            {
                return false; //never is called so this is not an issue
            }

            public static bool NobliumFormationReactionDelegate(GasMixture air_contents)
            {
                return ReactionFunctions.NobliumFormation(air_contents);
            }

            public static bool StimulumFormationReactionDelegate(GasMixture air_contents)
            {
                return ReactionFunctions.StimulumFormation(air_contents);
            }

            public static bool BZFormationReactionDelegate(GasMixture air_contents)
            {
                return ReactionFunctions.BZFormation(air_contents);
            }

            public static bool NitrylFormationReactionDelegate(GasMixture air_contents)
            {
                return ReactionFunctions.NitrylFormation(air_contents);
            }

            public static bool FusionReactionDelegate(GasMixture air_contents)
            {
                return ReactionFunctions.Fusion(air_contents);
            }

            public static bool FireReactionDelegate(GasMixture air_contents)
            {
                return ReactionFunctions.Fire(air_contents);
            }

            public static bool SterilizationDelegate(GasMixture air_contents)
            {
                return ReactionFunctions.Sterilization(air_contents);
            }
        }
        /// <summary>
        /// Contains implementation of static class GasReactionDictionaries that contains all requirement dictionaries for <see cref="Reactions"/>
        /// </summary>
        public static class GasReactionDictionaries
        {
            public static Dictionary<string, string> NobliumSuppressionRequirementDictionary = new Dictionary<string, string>
            {
                {"hypernob", $"{Num.ReactionOppressionThreshold}"}
            };
            public static Dictionary<string, string> NobliumFormationRequirementDictionary = new Dictionary<string, string>
            {
                {"TEMP",    $"{5000000}"},
                {"n2",      $"{10}"},
                {"tritium", $"{5}"}
            };
            public static Dictionary<string, string> StimulumFormationRequirementDictionary = new Dictionary<string, string>
            {
                {"TEMP",    $"{Num.StimulumHeatScale / 2}"},
                {"tritium", $"{30}"},
                {"plasma",  $"{10}"},
                {"bz",      $"{20}"},
                {"no2",     $"{30}"}
            };
            public static Dictionary<string, string> BZFormationRequirementDictionary = new Dictionary<string, string>
            {
                {"plasma", $"{10}"},
                {"n2o",  $"{10}"}
            };
            public static Dictionary<string, string> NitrylFormationRequirementDictionary = new Dictionary<string, string>
            {
                {"TEMP",    $"{Num.FireMinimumTemperature * 400}"},
                {"o2",      $"{20}"},
                {"n2",      $"{20}"},
                {"n2o",     $"{5}"}
            };
            public static Dictionary<string, string> FusionRequirementDictionary = new Dictionary<string, string>
            {
                //have to do 1000UL because it literally overflows at compile time
                {"TEMP",    $"{Num.FusionTemperatureThreshold}"},
                {"tritium", $"{Num.FusionTritiumMolesUsed}"},
                {"plasma",  $"{250}"},
                {"co2", $"{250}"},
            };
            public static Dictionary<string, string> FireRequirementDictionary = new Dictionary<string, string>
            {
                {"TEMP",    $"{Num.FireMinimumTemperature}"}
            };
            public static Dictionary<string, string> SterilizationRequirementDictionary = new Dictionary<string, string>
            {
                {"TEMP",    $"{Num.FireMinimumTemperature + 70}"},
                {"miasma",  $"{Num.MinimumMoleCount}"}
            };
        }
        /// <summary>
        /// Contains implementation of static class Reactions that contains all definitions for reactions (fire, nitryl formation, etc.).
        /// </summary>
        public static class Reactions
        {
            public static GasReaction NobliumSuppression = new GasReaction
                (
                "Hyper-Noblium Reaction Suppression",
                "nobstop",
                GasReactionDelegates.NobliumSuppressionReactionDelegate,
                GasReactionDictionaries.NobliumSuppressionRequirementDictionary,
                int.MaxValue
                );

            public static GasReaction NobliumFormation = new GasReaction
                (
                "Hyper-Noblium Condensation",
                "nobformation",
                GasReactionDelegates.NobliumFormationReactionDelegate,
                GasReactionDictionaries.NobliumFormationRequirementDictionary,
                6
                );

            public static GasReaction StimulumFormation = new GasReaction
                (
                "Stimulum Formation",
                "stimformation",
                GasReactionDelegates.StimulumFormationReactionDelegate,
                GasReactionDictionaries.StimulumFormationRequirementDictionary,
                5
                );

            public static GasReaction BZFormation = new GasReaction
                (
                "BZ Formation",
                "bzformation",
                GasReactionDelegates.BZFormationReactionDelegate,
                GasReactionDictionaries.BZFormationRequirementDictionary,
                4
                );

            public static GasReaction NitrylFormation = new GasReaction
                (
                "Nitryl Formation",
                "nitrylformation",
                GasReactionDelegates.NitrylFormationReactionDelegate,
                GasReactionDictionaries.NitrylFormationRequirementDictionary,
                3
                );

            //:clap: Fusion :clap: is :clap back :clap:
            public static GasReaction Fusion = new GasReaction
                (
                "Plasmic Fusion",
                "fusion",
                GasReactionDelegates.FusionReactionDelegate,
                GasReactionDictionaries.FusionRequirementDictionary,
                2
                );

            public static GasReaction Fire = new GasReaction
                (
                "Hydrocarbon Combustion",
                "fire",
                GasReactionDelegates.FireReactionDelegate,
                GasReactionDictionaries.FireRequirementDictionary,
                -1
                );

            public static GasReaction Sterilization = new GasReaction
                (
                "Dry Heat Sterilization",
                "sterilization",
                GasReactionDelegates.SterilizationDelegate,
                GasReactionDictionaries.SterilizationRequirementDictionary,
                -10
                );

            // TODO: Add support for pluoxium creation through radpulses on turfs

            /// <summary>
            /// All static reactions in List<GasReaction>
            /// </summary>
            public static List<GasReaction> ReactionList
            {
                get
                {
                    //see above for explanation on this
                    return typeof(Reactions).GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(g => (GasReaction)g.GetValue(null))
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Contains all definitions for contant numbers (int, double, etc.).
        /// </summary>
        public static class Num
        {
            //Important ones first
            public const double IdealGas = 8.31;
            public const double OneAtmosphere = 101.325;
            public const double TempMinimum = 2.7;
            public const double ZeroCelsius = 273.15;
            public const double TwentyCelsius = 293.15;
            public const double TankLeakPressure = 30 * OneAtmosphere;
            public const double TankRupturePressure = 35 * OneAtmosphere;
            public const double TankFragmentPresssure = 40 * OneAtmosphere;
            public const double TankFragmentScale = 6 * OneAtmosphere;
            public const double MinimumTempDelta = 0.5;
            public const double MinimumHeatCapacity = 0.0003;
            public const double MinimumMoleCount = 0.01;
            public const int    TankVolume = 70;
            public const int    AirPumpVolume = 1000;
            public const int    CanisterVolume = 1000;
            public const int    TurfVolume = 2500;

            //Plasma fire props
            public const double OxyBurnRateBase = 1.4;
            public const double PlasmaBurnRateDelta = 9;
            public const double PlasmaUpperTemperature = 1370 + ZeroCelsius;
            public const double FireMinimumTemperature = 100 + ZeroCelsius;
            public const int    PlasmaMinimumOxyNeeded = 2;
            public const int    PlasmaMinimumOxyPlasmaRatio = 30;
            public const int    PlasmaOxyFullburn = 10;
            public const int    CarbonEnergyReleased = 100000;
            public const int    HydrogenEnergyReleased = 280000;
            public const int    PlasmaEnergyReleased = 3000000;

            //Assmos props
            public const int    NitrylFormationEnergy = 100000;
            public const int    TritiumBurnOxyFactor = 100;
            public const int    TritiumBurnTritFactor = 10;
            public const int    TritiumBurnRadioactivityFactor = 50000;
            public const int    MinimumTritOxyBurnEnergy = 2000000;
            public const int    SuperSaturationThreshold = 96;
            public const double TritiumMinimumRadiationEnergy = 0.1;

            //Stimulum
            public const int    StimulumHeatScale = 100000;
            public const double StimulumFirstRise = 0.65;
            public const double StimulumFirstDrop = 0.065;
            public const double StimulumSecondRise = 0.0009;
            public const double StimulumAbsoluteDrop = 0.00000335;

            //Other
            public const int ReactionOppressionThreshold = 5;
            public const double NobliumFormationEnergy = 2e9;

            //Fusion
            public const double FusionEnergyThreshold = 3e9;
            public const double FusionTritConversionCoeff = 1e-10;
            public const double InstabilityGasPowerFactor = 0.003;
            public const double PlasmaBindingEnergy = 2e7;
            public const int FusionMoleThreshold = 250;
            public const int FusionTritiumMolesUsed = 1;
            public const int ToroidVolumeBreakeven = 1000;
            public const int FusionTemperatureThreshold = 10000;
            public const int FusionInstabilityEndothermality = 2;

            // For holder types
            public const int Turf = 0;
            public const int Tank = 1;
            public const int Pipe = 2;
            public const int Canister = 3;
        }
    }
}
