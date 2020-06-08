using MatchRetriever.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MatchRetriever.Misplays;
using MatchEntities;
using Microsoft.EntityFrameworkCore;

namespace MatchRetriever.ModelFactories
{
    public interface IMatchDataSetFactory
    {
        Task<MatchDataSet> GetModel(long matchId);
    }

    public class MatchDataSetFactory : ModelFactoryBase, IMatchDataSetFactory
    {

        public MatchDataSetFactory(IServiceProvider sp) : base(sp)
        {
        }


        public async Task<MatchDataSet> GetModel(long matchId)
        {
            var dataSet = new MatchDataSet();

            dataSet.MatchStats = await _context.MatchStats.FindAsync(matchId);
            dataSet.OverTimeStats = await _context.OverTimeStats.FindAsync(matchId);

            dataSet.PlayerMatchStatsList = await _context.PlayerMatchStats
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.RoundStatsList = await _context.RoundStats
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.PlayerRoundStatsList = await _context.PlayerRoundStats
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.BombPlantList = await _context.BombPlant
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.BombDefusedList = await _context.BombDefused
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.BombExplosionList = await _context.BombExplosion
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.BotTakeOverList = await _context.BotTakeOver
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.ConnectDisconnectList = await _context.ConnectDisconnect
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.HostageDropList = await _context.HostageDrop
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.HostagePickUpList = await _context.HostagePickUp
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.HostageRescueList = await _context.HostageRescue
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.ItemDroppedList = await _context.ItemDropped
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.ItemPickedUpList = await _context.ItemPickedUp
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.ItemSavedList = await _context.ItemSaved
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.RoundItemList = await _context.RoundItem
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.PlayerPositionList = await _context.PlayerPosition
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.DecoyList = await _context.Decoy
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.FireNadeList = await _context.FireNade
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.FlashList = await _context.Flash
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.FlashedList = await _context.Flashed
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.HeList = await _context.He
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.SmokeList = await _context.Smoke
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.DamageList = await _context.Damage
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.KillList = await _context.Kill
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.WeaponReloadList = await _context.WeaponReload
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            dataSet.WeaponFiredList = await _context.WeaponFired
                .Where(x => x.MatchId == matchId)
                .ToListAsync()
                .ConfigureAwait(false);

            return dataSet;
        }
    }
}
