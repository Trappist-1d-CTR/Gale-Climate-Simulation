using System.Reflection.Metadata.Ecma335;

namespace Gale_Climate_Model;

class Program
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
        public static bool AtDataStep()
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
        /// <summary>
        /// Returns whether the simulation time is within limits.
        /// </summary>
        /// <returns>True if the time is not above the maximum, False otherwise</returns>
        public static bool WithinTimeLimit()
        {
            return Time + HalfStep < MaxTimeLimit;
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
        /// Molar Gas Constant. Unit: J / mol K
        /// </summary>
        public const double R = 8.31446261815324;
        /// <summary>
        /// 1/2 is Universally constant. Unit: none
        /// </summary>
        public const double Zeta = 1 / 2;
    }

    /// <summary>
    /// Class containing reference values, often useful as units (AU, Earth radii, etc).
    /// </summary>
    public static class Unit
    {
        /// <summary>
        /// The Radius of the Earth, as per SI standard. Unit: meters
        /// </summary>
        public const double EarthRadii = 6.3781e6;
    }

    /// <summary>
    /// Class containing all of the values that are input, important middle values, and output.
    /// </summary>
    public static class Data
    {
        //Input
        /// <summary>
        /// Mean Radius: the planet's average radius. Unit: meters
        /// </summary>
        public static double GaleR = 2 * Unit.EarthRadii;
        /// <summary>
        /// Albedo: percentage of incoming radiation that gets reflected out the system. Unit: none
        /// </summary>
        public static double GaleAlbedo = 0.9;
        /// <summary>
        /// Greenhouse: percentage of outgoing radiation that gets reflected into the system. Unit: none
        /// </summary>
        public static double GaleGreenhouse = 0.8;
        /// <summary>
        /// Insolation: amount of radiation the planet receives from its star. Unit: W / m^2
        /// </summary>
        public static double Insolation = 5000;
        /// <summary>
        /// Planetary Heat Capacity: amount of heat required to hear up the planet by 1 K. Unit: J / K
        /// </summary>
        public static double GaleHeatCapacity = 5000 * 4 * Math.PI * Math.Pow(Data.GaleR, 2);

        //Calculations (Middle and Output)
        /// <summary>
        /// Incoming Energy in the planetary system. Unit: Joules
        /// </summary>
        public static double EnergyIn = 0;
        /// <summary>
        /// Outgoing Energy in the planetary system. Unit: Joules
        /// </summary>
        public static double EnergyOut = 0;
        /// <summary>
        /// Mean Temperature in the planetary system. Unit: Kelvin
        /// </summary>
        public static double MeanTemperature = 0;
    }

    #endregion

    /// <summary>
    /// Main script of the program.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Console.WriteLine("Simulation Begin\nTime: " + Sim.Time + "\nMeanT: " + Data.MeanTemperature);
        Console.ReadKey();

        while (Sim.WithinTimeLimit())
        {
            SimulationTimeStep();

            if (Sim.AtDataStep())
            {
                Console.WriteLine("Simulation Step Completed\nTime: " + Sim.Time);
                Console.WriteLine("MeanT: " + Data.MeanTemperature);
                Console.WriteLine("Ein: " + Data.EnergyIn);
                Console.WriteLine("Eout: " + Data.EnergyOut);
                Console.ReadKey();
            }
        }
    }

    /// <summary>
    /// Performs the time step of the simulation.
    /// </summary>
    public static void SimulationTimeStep()
    {
        Data.EnergyIn = Data.Insolation * (1 - Data.GaleAlbedo) * Math.PI * Math.Pow(Data.GaleR, 2);
        Data.EnergyOut = 4 * Math.PI * Math.Pow(Data.GaleR, 2) * Data.GaleGreenhouse * Const.StefBoltz * Math.Pow(Data.MeanTemperature, 4);

        Data.MeanTemperature += (Data.EnergyIn - Data.EnergyOut) / Data.GaleHeatCapacity * Sim.TimeStep;

        Sim.Step();
    }
}
