using System;

namespace BuddieMain
{
    public class GameSaveData : IEquatable<GameSaveData>
    {
        public bool _Level2;
        public bool _Level3;
        public int _ClothesShirt;
        public int _ClothesPants;
        public int _ClothesShoes;

        public GameSaveData()
        {
            _Level2 = false;
            _Level3 = false;
            _ClothesShirt = 0;
            _ClothesPants = 0;
            _ClothesShoes = 0;
        }
                
        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4}", _Level2, _Level3, _ClothesShirt, _ClothesPants, _ClothesShoes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(GameSaveData)) return false;
            return Equals((GameSaveData)obj);
        }

        public bool Equals(GameSaveData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._Level2 == _Level2 && other._Level3 == _Level3;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _ClothesShirt;
                result = (result * 397) ^ _ClothesPants.GetHashCode();
                result = (result * 397) ^ _ClothesShoes.GetHashCode();
                return result;
            }
        }

        public static bool operator ==(GameSaveData left, GameSaveData right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GameSaveData left, GameSaveData right)
        {
            return !Equals(left, right);
        }
    }
}
