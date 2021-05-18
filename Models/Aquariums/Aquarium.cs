using AquaShop.Models.Aquariums.Contracts;
using AquaShop.Models.Decorations.Contracts;
using AquaShop.Models.Fish.Contracts;
using AquaShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AquaShop.Models.Aquariums
{
    public abstract class Aquarium : IAquarium
    {
        private string name;
        private int capacity;

        private List<IDecoration> decorations;
        private List<IFish> fishes;

        protected Aquarium(string name, int capacity)
        {
            this.Name = name;
            this.Capacity = capacity;
            this.decorations = new List<IDecoration>();
            this.fishes = new List<IFish>();
        }

        public string Name
        {
            get => this.name;
            private set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(ExceptionMessages.InvalidAquariumName);
                }

                this.name = value;
            }
        }

        public int Capacity
        {
            get => this.capacity;
            private set => this.capacity = value;
        }


        public int Comfort => this.decorations.Sum(d => d.Comfort);

        public ICollection<IDecoration> Decorations => this.decorations;

        public ICollection<IFish> Fish => this.fishes;


        public void AddFish(IFish fish)
        {
            if (this.fishes.Count + 1 > this.Capacity)
            {
                throw new ArgumentException(ExceptionMessages.NotEnoughCapacity);
            }

            this.fishes.Add(fish);
        }
        public bool RemoveFish(IFish fish)
        {
            return this.fishes.Remove(fish);
        }

        public void AddDecoration(IDecoration decoration)
        {
            this.decorations.Add(decoration);
        }
        public void Feed()
        {
            foreach (var fish in this.fishes)
            {
                fish.Eat();
            }
        }

        public string GetInfo()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{this.Name} ({this.GetType().Name}):");

            if (!this.fishes.Any())
            {
                sb.AppendLine("Fish: none");
            }
            else
            {
                sb.Append("Fish: ");
                sb.Append(string.Join(", ", this.fishes.Select(f => f.Name)) + Environment.NewLine);           
            }

            sb.AppendLine($"Decorations: {this.decorations.Count}");
            sb.AppendLine($"Comfort: {this.Comfort}");

            return sb.ToString().TrimEnd();
        }

    }
}
