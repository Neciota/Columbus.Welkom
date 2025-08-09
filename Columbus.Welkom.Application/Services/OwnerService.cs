using Columbus.Models.Owner;
using Columbus.Models.Pigeon;
using Columbus.UDP.Interfaces;
using Columbus.Welkom.Application.Models.Entities;
using Columbus.Welkom.Application.Providers;
using Columbus.Welkom.Application.Repositories.Interfaces;
using Columbus.Welkom.Application.Services.Interfaces;

namespace Columbus.Welkom.Application.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IFilePicker _filePicker;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPigeonRepository _pigeonRepository;
        private readonly IOwnerSerializer _ownerSerializer;

        public OwnerService(IOwnerRepository ownerRepository, IPigeonRepository pigeonRepository, IFilePicker filePicker, IOwnerSerializer ownerSerializer)
        {
            _filePicker = filePicker;
            _ownerRepository = ownerRepository;
            _pigeonRepository = pigeonRepository;
            _ownerSerializer = ownerSerializer;
        }

        public async Task<IEnumerable<Owner>> ReadOwnersFromFileAsync()
        {
            (StreamReader? stream, string fileName) = await _filePicker.OpenFileAsync([".udp"]);
            if (stream is null)
                return [];

            return await _ownerSerializer.DeserializeAsync(stream);
        }

        public async Task<IEnumerable<Owner>> GetOwnersWithAllPigeonsAsync()
        {
            IEnumerable<OwnerEntity> owners = await _ownerRepository.GetAllWithAllPigeonsAsync();

            return owners.Select(o => o.ToOwner());
        }

        public async Task<IEnumerable<Owner>> GetOwnersWithYearPigeonsAsync(int year, bool includeOwnersWithoutPigeons = false)
        {
            IEnumerable<OwnerEntity> owners = await _ownerRepository.GetAllWithPigeonsForYearAsync(year, includeOwnersWithoutPigeons);

            return owners.Select(o => o.ToOwner());
        }

        public async Task UpdateOwnersAsync(IEnumerable<Owner> owners)
        {
            IEnumerable<OwnerEntity> currentOwners = await _ownerRepository.GetAllAsync();

            IEnumerable<OwnerEntity> ownersToUpdate = currentOwners.IntersectBy(owners.Select(o => o.Id), o => o.OwnerId);
            IEnumerable<OwnerEntity> ownersToDelete = currentOwners.ExceptBy(owners.Select(o => o.Id), o => o.OwnerId);
            IEnumerable<OwnerEntity> ownersToAdd = owners.ExceptBy(currentOwners.Select(o => o.OwnerId), o => o.Id)
                .Select(o => new OwnerEntity(o));

            foreach ((OwnerEntity ownerToUpdate, Owner owner) in ownersToUpdate.Join(owners, o => o.OwnerId, o => o.Id, (oe, o) => (oe, o)))
            {
                ownerToUpdate.Club = owner.Club;
                ownerToUpdate.Name = owner.Name;
                ownerToUpdate.Latitude = owner.LoftCoordinate.Lattitude;
                ownerToUpdate.Longitude = owner.LoftCoordinate.Longitude;
            }

            await _ownerRepository.UpdateRangeAsync(ownersToUpdate);
            await _ownerRepository.AddRangeAsync(ownersToAdd);
            await _ownerRepository.DeleteRangeAsync(ownersToDelete);

            IEnumerable<PigeonEntity> currentPigeons = await _pigeonRepository.GetAllAsync();
            IEnumerable<Pigeon> pigeons = owners.SelectMany(o => o.Pigeons);
            Dictionary<PigeonId, OwnerId> ownerIdsByPigeonId = owners.SelectMany(o => o.Pigeons.Select(p => (p.Id, o.Id))).ToDictionary(po => po.Item1, po => po.Item2);

            IEnumerable<PigeonEntity> pigeonsToUpdate = currentPigeons.IntersectBy(pigeons.Select(p => p.Id), p => p.Id);
            IEnumerable<PigeonEntity> pigeonsToDelete = currentPigeons.ExceptBy(pigeons.Select(p => p.Id), p => p.Id);
            IEnumerable<PigeonEntity> pigeonsToAdd = pigeons.ExceptBy(currentPigeons.Select(p => p.Id), p => p.Id)
                .Select(p => new PigeonEntity(p, ownerIdsByPigeonId[p.Id]));

            foreach ((PigeonEntity pigeonToUpdate, Pigeon pigeon) in pigeonsToUpdate.Join(pigeons, p => p.Id, p => p.Id, (pe, p) => (pe, p)))
            {
                pigeonToUpdate.Chip = pigeon.Chip;
                pigeonToUpdate.Sex = pigeon.Sex;
                pigeonToUpdate.OwnerId = ownerIdsByPigeonId[pigeon.Id];
            }

            await _pigeonRepository.UpdateRangeAsync(pigeonsToUpdate);
            await _pigeonRepository.DeleteRangeAsync(pigeonsToDelete);
            await _pigeonRepository.AddRangeAsync(pigeonsToAdd);
        }
    }
}
