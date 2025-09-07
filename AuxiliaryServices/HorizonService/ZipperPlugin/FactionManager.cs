using System;

namespace HorizonService.ZipperPlugin
{
    // Enable/disable a faction by index (0 to 31)
    public class FactionManager
    {
        private uint _factionMask;

        public FactionManager(uint mask = 0)
        {
            _factionMask = mask;
        }

        public void EnableFaction(int factionIndex)
        {
            if (factionIndex < 0 || factionIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(factionIndex), "Faction index must be between 0 and 31.");

            _factionMask |= 1u << factionIndex;
        }

        public void DisableFaction(int factionIndex)
        {
            if (factionIndex < 0 || factionIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(factionIndex), "Faction index must be between 0 and 31.");

            _factionMask &= ~(1u << factionIndex);
        }

        public bool IsFactionEnabled(int factionIndex)
        {
            if (factionIndex < 0 || factionIndex >= 32)
                throw new ArgumentOutOfRangeException(nameof(factionIndex), "Faction index must be between 0 and 31.");

            return (_factionMask & 1u << factionIndex) != 0;
        }

        public uint GetMask()
        {
            return _factionMask;
        }
    }
}
