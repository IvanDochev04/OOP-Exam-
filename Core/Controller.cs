

using AquaShop.Core.Contracts;
using AquaShop.Models.Aquariums;
using AquaShop.Models.Aquariums.Contracts;
using AquaShop.Models.Decorations;
using AquaShop.Models.Decorations.Contracts;
using AquaShop.Models.Fish;
using AquaShop.Repositories;
using AquaShop.Utilities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AquaShop.Core
{
    public class Controller : IController
    {
        private DecorationRepository decorations;
        private List<IAquarium> aquariums;

        public Controller()
        {
            this.decorations = new DecorationRepository();
            this.aquariums = new List<IAquarium>();
        }

        public string AddAquarium(string aquariumType, string aquariumName)
        {
            IAquarium aquarium = null;

            switch (aquariumType)
            {
                case "FreshwaterAquarium":
                    aquarium = new FreshwaterAquarium(aquariumName);
                    break;
                case "SaltwaterAquarium":
                    aquarium = new SaltwaterAquarium(aquariumName);
                    break;
                default:
                    throw new InvalidOperationException(ExceptionMessages.InvalidAquariumType);
            }

            this.aquariums.Add(aquarium);

            return string.Format(OutputMessages.SuccessfullyAdded, aquariumType);
        }

        public string AddDecoration(string decorationType)
        {
            IDecoration decoration = null;

            switch (decorationType)
            {
                case "Ornament":
                    decoration = new Ornament();
                    break;
                case "Plant":
                    decoration = new Plant();
                    break;
                default:
                    throw new InvalidOperationException(ExceptionMessages.InvalidDecorationType);
            }

            this.decorations.Add(decoration);

            return string.Format(OutputMessages.SuccessfullyAdded, decorationType);
        }

        public string InsertDecoration(string aquariumName, string decorationType)
        {
            var decorationToInsert = this.decorations.FindByType(decorationType);

            if (decorationToInsert == null)
            {
                throw new InvalidOperationException(string.Format(ExceptionMessages.InexistentDecoration, decorationType));
            }

            var aquariumToAdd = this.aquariums.FirstOrDefault(a => a.Name == aquariumName);

            aquariumToAdd.Decorations.Add(decorationToInsert);

            this.decorations.Remove(decorationToInsert);

            return string.Format(OutputMessages.EntityAddedToAquarium, decorationType, aquariumName);
        }

        public string AddFish(string aquariumName, string fishType, string fishName, string fishSpecies, decimal price)
        {
            Fish fishToAdd = null;

            switch (fishType)
            {
                case "FreshwaterFish":
                    fishToAdd = new FreshwaterFish(fishName, fishSpecies, price);
                    break;
                case "SaltwaterFish":
                    fishToAdd = new SaltwaterFish(fishName, fishSpecies, price);
                    break;
                default:
                    throw new InvalidOperationException(ExceptionMessages.InvalidFishType);
            }

            var desiredAquarium = this.aquariums.FirstOrDefault(a => a.Name == aquariumName);

            if (desiredAquarium.GetType().Name != fishToAdd.LivingWater)
            {
                return OutputMessages.UnsuitableWater;
            }

            desiredAquarium.AddFish(fishToAdd);

            return string.Format(OutputMessages.EntityAddedToAquarium, fishType, aquariumName);
        }

        public string FeedFish(string aquariumName)
        {
            var aquarium = this.aquariums.FirstOrDefault(a => a.Name == aquariumName);

            foreach (var fish in aquarium.Fish)
            {
                fish.Eat();
            }

            return string.Format(OutputMessages.FishFed, aquarium.Fish.Count);
        }

        public string CalculateValue(string aquariumName)
        {
            var aquarium = this.aquariums.FirstOrDefault(a => a.Name == aquariumName);

            var fishPrice = 0m;
            var decorationPrice = 0m;

            foreach (var fish in aquarium.Fish)
            {
                fishPrice += fish.Price;
            }
            foreach (var decoration in aquarium.Decorations)
            {
                decorationPrice += decoration.Price;
            }

            decimal aquariumValue = fishPrice + decorationPrice;

            //return string.Format(OutputMessages.AquariumValue, aquariumValue);
            return $"The value of Aquarium {aquariumName} is {aquariumValue:f2}.";
        }

        public string Report()
        {
            var sb = new StringBuilder();

            foreach (var aquarium in this.aquariums)
            {
                sb.AppendLine(aquarium.GetInfo());
            }

            return sb.ToString().TrimEnd();
        }
    }
}
