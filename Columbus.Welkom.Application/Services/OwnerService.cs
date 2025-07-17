using Columbus.Models.Owner;
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
            StreamReader? stream = await _filePicker.OpenFileAsync([".udp"]);
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

        public async Task OverwriteOwnersAsync(IEnumerable<Owner> owners)
        {
            IEnumerable<OwnerEntity> currentOwners = await _ownerRepository.GetAllAsync();
            await _ownerRepository.DeleteRangeAsync(currentOwners);

            List<OwnerEntity> ownerEntities = owners.Select(o => new OwnerEntity(o))
                .ToList();
            IEnumerable<OwnerEntity> addedOwners = await _ownerRepository.AddRangeAsync(ownerEntities);
            Dictionary<OwnerId, OwnerEntity> addedOwnersByOwnerId = addedOwners.ToDictionary(ao => ao.OwnerId);

            List<PigeonEntity> pigeonEntities = owners.SelectMany(o => o.Pigeons.Select(p => new PigeonEntity(p, addedOwnersByOwnerId[o.Id].OwnerId)))
                .ToList();
            await _pigeonRepository.AddRangeAsync(pigeonEntities);
        }
    }
}
