using BestiaryArenaCracker.Domain;
using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.Repository.Context;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.EntityFrameworkCore;

namespace BestiaryArenaCracker.ML.Models;

public class ScoreModelInput
{
    [VectorType]
    public float[] Features { get; set; } = [];
    public float Points { get; set; }
}

public class SpeedModelInput
{
    [VectorType]
    public float[] Features { get; set; } = [];
    public float Ticks { get; set; }
}

public class ScoreModelOutput
{
    public float Score { get; set; }
}

public class SpeedModelOutput
{
    public float Score { get; set; }
}

public class CompositionModelTrainer(ApplicationDbContext dbContext)
{
    private readonly MLContext _mlContext = new();
    private readonly ApplicationDbContext _dbContext = dbContext;
    private int _maxTeamSize = 1;

    public int MaxTeamSize => _maxTeamSize;

    public ITransformer TrainScoreModel(string? modelPath = null)
    {
        var data = LoadScoreData();
        var dataView = _mlContext.Data.LoadFromEnumerable(data);
        var pipeline = _mlContext.Transforms.NormalizeMinMax("Features")
            .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(ScoreModelInput.Points), featureColumnName: "Features"));
        var model = pipeline.Fit(dataView);
        if (modelPath != null)
            _mlContext.Model.Save(model, dataView.Schema, modelPath);
        return model;
    }

    public ITransformer TrainSpeedModel(string? modelPath = null)
    {
        var data = LoadSpeedData();
        var dataView = _mlContext.Data.LoadFromEnumerable(data);
        var pipeline = _mlContext.Transforms.NormalizeMinMax("Features")
            .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: nameof(SpeedModelInput.Ticks), featureColumnName: "Features"));
        var model = pipeline.Fit(dataView);
        if (modelPath != null)
            _mlContext.Model.Save(model, dataView.Schema, modelPath);
        return model;
    }

    private List<ScoreModelInput> LoadScoreData()
    {
        var bestResults = _dbContext.CompositionResults.AsNoTracking()
            .Where(r => r.Victory)
            .GroupBy(r => r.CompositionId)
            .Select(g => g.OrderByDescending(r => r.Points).ThenBy(r => r.Ticks)
                .Select(r => new { r.CompositionId, r.Points })
                .First())
            .ToList();

        var compositionIds = bestResults.Select(r => r.CompositionId).ToList();

        var monsters = _dbContext.CompositionMonsters.AsNoTracking()
            .Where(m => compositionIds.Contains(m.CompositionId))
            .ToList();

        _maxTeamSize = _dbContext.CompositionMonsters.AsNoTracking()
            .GroupBy(m => m.CompositionId)
            .Select(g => g.Count())
            .Max();

        var groups = monsters.GroupBy(m => m.CompositionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var list = new List<ScoreModelInput>(bestResults.Count);
        foreach (var result in bestResults)
        {
            if (groups.TryGetValue(result.CompositionId, out var team))
            {
                var features = FlattenMonsters(team, _maxTeamSize);
                list.Add(new ScoreModelInput { Features = features, Points = result.Points });
            }
        }

        return list;
    }

    private List<SpeedModelInput> LoadSpeedData()
    {
        var bestResults = _dbContext.CompositionResults.AsNoTracking()
            .Where(r => r.Victory)
            .GroupBy(r => r.CompositionId)
            .Select(g => g.OrderBy(r => r.Ticks)
                .Select(r => new { r.CompositionId, r.Ticks })
                .First())
            .ToList();

        var compositionIds = bestResults.Select(r => r.CompositionId).ToList();

        var monsters = _dbContext.CompositionMonsters.AsNoTracking()
            .Where(m => compositionIds.Contains(m.CompositionId))
            .ToList();

        _maxTeamSize = _dbContext.CompositionMonsters.AsNoTracking()
            .GroupBy(m => m.CompositionId)
            .Select(g => g.Count())
            .Max();

        var groups = monsters.GroupBy(m => m.CompositionId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var list = new List<SpeedModelInput>(bestResults.Count);
        foreach (var result in bestResults)
        {
            if (groups.TryGetValue(result.CompositionId, out var team))
            {
                var features = FlattenMonsters(team, _maxTeamSize);
                list.Add(new SpeedModelInput { Features = features, Ticks = result.Ticks });
            }
        }

        return list;
    }

    private static float[] FlattenMonsters(List<CompositionMonstersEntity> monsters, int maxTeamSize)
    {
        var features = new List<float>(maxTeamSize * 11);
        foreach (var m in monsters)
        {
            var name = Enum.TryParse<Creatures>(m.Name, out var c) ? (float)(int)c : 0f;
            features.Add(name);
            features.Add(m.Hitpoints);
            features.Add(m.Attack);
            features.Add(m.AbilityPower);
            features.Add(m.Armor);
            features.Add(m.MagicResistance);
            features.Add(m.Level);
            features.Add(m.TileLocation);
            features.Add((float)m.Equipment);
            features.Add((float)m.EquipmentStat);
            features.Add(m.EquipmentTier);
        }

        var padding = maxTeamSize - monsters.Count;
        for (int i = 0; i < padding * 11; i++)
        {
            features.Add(0f);
        }

        return [.. features];
    }

    public static float[] Flatten(List<CompositionMonstersEntity> monsters, int maxTeamSize)
        => FlattenMonsters(monsters, maxTeamSize);
}

public class CompositionModelPredictor
{
    private readonly MLContext _mlContext = new();
    private readonly PredictionEngine<ScoreModelInput, ScoreModelOutput> _scoreEngine;
    private readonly PredictionEngine<SpeedModelInput, SpeedModelOutput> _speedEngine;
    private readonly int _maxTeamSize;

    public CompositionModelPredictor(ITransformer scoreModel, ITransformer speedModel, int maxTeamSize)
    {
        _maxTeamSize = maxTeamSize;
        _scoreEngine = _mlContext.Model.CreatePredictionEngine<ScoreModelInput, ScoreModelOutput>(scoreModel);
        _speedEngine = _mlContext.Model.CreatePredictionEngine<SpeedModelInput, SpeedModelOutput>(speedModel);
    }

    public float PredictScoreBasedPerformance(List<CompositionMonstersEntity> monsters)
    {
        var input = new ScoreModelInput { Features = CompositionModelTrainer.Flatten(monsters, _maxTeamSize) };
        return _scoreEngine.Predict(input).Score;
    }

    public float PredictSpeedBasedPerformance(List<CompositionMonstersEntity> monsters)
    {
        var input = new SpeedModelInput { Features = CompositionModelTrainer.Flatten(monsters, _maxTeamSize) };
        return _speedEngine.Predict(input).Score;
    }
}
