using System;

namespace Gale_Climate_Model;

public sealed class Program
{
    #region Variables

    /// <summary>
    /// Class which contains and manages simulation values, such as time.
    /// </summary>
    public static class Sim
    {
        /// <summary>
        /// Current time of the simulation. Unit: seconds
        /// </summary>
        public static double Time = 0;

        /// <summary>
        /// Time step of the simulation. Unit:seconds
        /// </summary>
        public const double TimeStep = 1;

        /// <summary>
        /// Time step at which data is shown on the console. Unit: seconds
        /// </summary>
        public const double DataStep = 2e3;

        /// <summary>
        /// Half of the time step, used to check for time conditions in spit of floating point errors. Unit: seconds
        /// </summary>
        public const double HalfStep = TimeStep * Const.Zeta;
        /// <summary>
        /// Maximum simulation time the simulation is allowed to run for, prevents infinte loops. Unit: seconds
        /// </summary>
        public const double MaxTimeLimit = 1e5;

        #region Function-use variables
        private static double LastTimeSinceDataStep;
        #endregion

        /// <summary>
        /// Perform one time step.
        /// </summary>
        public static void Step()
        {
            Time += TimeStep;
            LastTimeSinceDataStep += TimeStep;
        }

        /// <summary>
        /// Returns whether the simulation time is currently at a data step.
        /// </summary>
        /// <returns>True if data should be shown, False otherwise</returns>
        public static bool AtDataStep
        {
            get
            {
                if (LastTimeSinceDataStep >= DataStep)
                {
                    LastTimeSinceDataStep -= DataStep;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns whether the simulation time is within limits.
        /// </summary>
        /// <returns>True if the time is not above the maximum, False otherwise</returns>
        public static bool WithinTimeLimit
        {
            get { return Time + HalfStep < MaxTimeLimit; }
        }
    }

    /// <summary>
    /// Class containing Universal constants.
    /// </summary>
    public static class Const
    {
        /// <summary>
        /// Speed of Light in a vacuum. Unit: meters per second
        /// </summary>
        public const double c = 299792458; // m / s

        /// <summary>
        /// Newtonian Constant of Gravitation. Unit: m^3 / kg s^2
        /// </summary>
        public const double G = 6.67430e-11; // m^3 / kg s^2

        /// <summary>
        /// Stefan-Boltzmann Constant. Unit: W / m^2 T^4
        /// </summary>
        public const double StefBoltz = 5.670374419e-8;

        /// <summary>
        /// Boltzmann Constant. Unity: J/K
        /// </summary>
        public const double Boltzmann = 1.380649e-23;

        /// <summary>
        /// Omega Constant. Unit: none
        /// </summary>
        public const double Omega = 0.567143290409783873;

        /// <summary>
        /// Molar Gas Constant. Unit: J / mol K
        /// </summary>
        public const double R = 8.31446261815324;

        /// <summary>
        /// Avogadro's Numer. Unit: none
        /// </summary>
        public const double NA = 6.02214076e23;

        /// <summary>
        /// 1/2 is Universally constant. Unit: none
        /// </summary>
        public const double Zeta = 1.0 / 2.0;
    }

    /// <summary>
    /// Class containing reference values, often useful as units (AU, Earth radii, etc).
    /// </summary>
    public static class Unit
    {
        /// <summary>
        /// Astrnomical Unit, equal to the semi-major axis of the Earth's orbit. Unit: m
        /// </summary>
        public const double AU = 1.495978707e11;

        public struct Earth
        {
            /// <summary>
            /// The Radius of the Earth, as per SI standard. Unit: m
            /// </summary>
            public const double Radius = 6.3781e6;

            /// <summary>
            /// The Mass of the Eartg, as per SI standard. Unit: kg
            /// </summary>
            public const double Mass = 5.9722e24;

            /// <summary>
            /// Earth's Atmospheric Pressure at sea level, as per SI standard. Unit: Pa = N / m^2
            /// </summary>
            public const double AtmPressure = 101325;

            /// <summary>
            /// Earth's average Amtospheric Heat Capacity. Unit: J / kg K
            /// </summary>
            public const double AtmHeatCapacity = 1006;
        }

        public struct Sun
        {
            /// <summary>
            /// The Mass of the Sun, as per SI standard. Unit: kg
            /// </summary>
            public const double Mass = 1.988416e30;

            /// <summary>
            /// The Radius of the Sun, as per SI standard. Unit: m
            /// </summary>
            public const double Radius = 6.957e8;

            /// <summary>
            /// The Temperature of the Sun's photosphere. Unit: K
            /// </summary>
            public const double SurfaceTemp = 5777;
        }
    }

    /// <summary>
    /// Class containing all of the values that are input, important middle values, and output.
    /// </summary>
    public static class Data
    {
        //  Input
        public struct Gale
        {
            /// <summary>
            /// Mean Radius: the planet's average radius. Unit: meters
            /// </summary>
            public static double Radius = 2 * Unit.Earth.Radius;

            /// <summary>
            /// Albedo: percentage of incoming radiation that gets reflected out the system. Unit: none
            /// </summary>
            public static double Albedo = 0.9;

            /// <summary>
            /// Greenhouse: percentage of outgoing radiation that gets reflected into the system. Unit: none
            /// </summary>
            public static double Greenhouse = 0.8;

            /// <summary>
            /// Insolation: amount of radiation the planet receives from its star. Unit: W / m^2
            /// </summary>
            public static double Insolation = 5000;

            /// <summary>
            /// Planetary Heat Capacity: amount of heat required to hear up the planet by 1 K. Unit: J / K
            /// </summary>
            public static double HeatCapacity = 5000 * 4 * Math.PI * Math.Pow(Data.Gale.Radius, 2);

            /// <summary>
            /// Mean Temperature: the planet's average surface temperature. Unit: Kelvin
            /// </summary>
            public static double MeanTemperature = 0;
        }

        //  Calculations (Middle and Output)
        /// <summary>
        /// Incoming Energy in the planetary system. Unit: Joules
        /// </summary>
        public static double EnergyIn = 0;

        /// <summary>
        /// Outgoing Energy in the planetary system. Unit: Joules
        /// </summary>
        public static double EnergyOut = 0;
    }

    #endregion

    /// <summary>
    /// Main function of the program.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.WriteLine("Press a key to begin the simulation.");
        Console.ReadKey();

        while (Sim.WithinTimeLimit)
        {
            if (Sim.AtDataStep)
                SimulationDisplayData();

            SimulationTimeStep();
        }
    }

    /// <summary>
    /// Performs the time step of the simulation.
    /// </summary>
    public static void SimulationTimeStep()
    {
        Data.EnergyIn = Data.Gale.Insolation * (1 - Data.Gale.Albedo) * Math.PI * Math.Pow(Data.Gale.Radius, 2);
        Data.EnergyOut = 4 * Math.PI * Math.Pow(Data.Gale.Radius, 2) * Data.Gale.Greenhouse * Const.StefBoltz * Math.Pow(Data.Gale.MeanTemperature, 4);

        Data.Gale.MeanTemperature += (Data.EnergyIn - Data.EnergyOut) / Data.Gale.HeatCapacity * Sim.TimeStep;

        Sim.Step();
    }

    /// <summary>
    /// Displays variables of the simulation.
    /// </summary>
    public static void SimulationDisplayData()
    {
        Console.WriteLine("Simulation Step Completed\nTime: " + Sim.Time);
        Console.WriteLine("MeanT: " + Data.Gale.MeanTemperature);
        Console.WriteLine("Ein: " + Data.EnergyIn);
        Console.WriteLine("Eout: " + Data.EnergyOut);
        Console.ReadKey();
    }
}
