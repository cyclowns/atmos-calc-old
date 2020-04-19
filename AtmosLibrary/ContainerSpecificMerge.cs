using System;

namespace AtmosLibrary
{
    /// <summary>
    /// Contains functions that merge two gasmixtures based on predefined conditions.
    /// Can merge TTV tanks, a canister/holding tank, or an airpump/holding tank
    /// </summary>
    public static class ContainerSpecificMerge
    {
        /// <summary>
        /// Merge function for two GasMixtures inside of a TTV. Calls the -true- merge() function.
        /// </summary>
        /// <param name="receiver"><see cref="GasMixture"/> that is returned merged with giver</param>
        /// <param name="giver"><see cref="GasMixture"/> that is merged with receiver</param>
        /// <returns>Merged <see cref="GasMixture"/></returns>
        public static GasMixture TTV_Merge(GasMixture receiver, GasMixture giver)
        {
            if (!receiver || !giver)
            {
                throw new ArgumentNullException("Null arguments passed to TTV_Merge()");
            }

            receiver.volume += giver.volume;
            receiver.merge(giver);

            return receiver;
        }

        /// <summary>
        /// Merge function for two GasMixtures, a canister and its holding tank. Calls the -true- merge() function.
        /// </summary>
        /// <param name="canister"><see cref="GasMixture"/> that is merged with tank, is the canister</param>
        /// <param name="tank"><see cref="GasMixture"/> that is merged with canister, is the returned tank</param>
        /// <param name="target_pressure">Target pressure of the canister</param>
        /// <returns>Merged <see cref="GasMixture"/></returns>
        //obsolete now.
        //public static GasMixture Canister_Merge(GasMixture canister, GasMixture tank, double target_pressure)
        //{
        //    if(!canister || !tank)
        //    {
        //        throw new ArgumentNullException("Null arguments passed to Canister_Merge()");
        //    }
        //    //Technically passive gate merge
        //    double tank_starting_pressure = tank.total_pressure(); //airs[2] output
        //    double canister_starting_pressure = canister.total_pressure(); //airs[1] input

        //    if(!(tank_starting_pressure >= Math.Min(target_pressure, canister_starting_pressure - 10)))
        //    {
        //        if((canister.total_moles() > 0) && (canister.temperature > 0))
        //        {
        //            double pressure_delta = Math.Min(target_pressure - tank_starting_pressure, (canister_starting_pressure - tank_starting_pressure) / 2);
        //            double transfer_moles = pressure_delta * tank.volume / (canister.temperature * Constants.Num.IdealGas);

        //            GasMixture removed = canister.remove(transfer_moles);
        //            tank.merge(removed);
        //        }
        //    }

        //    return tank;
        //}

        /// <summary>
        /// Merge function for two GasMixtures, an air pump and its holding tank. Calls the -true- merge() function.
        /// </summary>
        /// <param name="airpump"><see cref="GasMixture"/> that is merged with tank, is the airpump</param>
        /// <param name="tank"><see cref="GasMixture"/> that is merged with airpump, is the returned tank</param>
        /// <param name="target_pressure">Target pressure of the air pump</param>
        /// <returns>Merged <see cref="GasMixture"/></returns>
        public static GasMixture AirPump_Merge(GasMixture airpump, GasMixture tank, double target_pressure)
        {
            if (!airpump || !tank)
            {
                throw new ArgumentNullException("Null arguments passed to AirPump_Merge()");
            }

            double output_starting_pressure = tank.total_pressure();


            if ((target_pressure - output_starting_pressure) < 0.01)
            {
                return null;
            }

            if ((airpump.total_moles() > 0) && (airpump.temperature > 0))
            {
                double pressure_delta = target_pressure - output_starting_pressure;
                double transfer_moles = pressure_delta * tank.volume / (airpump.temperature * Constants.Num.IdealGas);

                GasMixture temp_removed = airpump.remove(transfer_moles);
                tank.merge(temp_removed);

                return tank;
            }

            return null;
        }
    }
}
