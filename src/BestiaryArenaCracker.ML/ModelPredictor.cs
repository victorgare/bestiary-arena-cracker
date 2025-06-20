using BestiaryArenaCracker.Domain.Entities;
using BestiaryArenaCracker.ML.Models;

namespace BestiaryArenaCracker.ML;

public class ModelPredictor
{
    private readonly CompositionModelPredictor _predictor;

    public ModelPredictor(CompositionModelPredictor predictor)
    {
        _predictor = predictor;
    }

    public float PredictScoreBasedPerformance(List<CompositionMonstersEntity> monsters)
        => _predictor.PredictScoreBasedPerformance(monsters);

    public float PredictSpeedBasedPerformance(List<CompositionMonstersEntity> monsters)
        => _predictor.PredictSpeedBasedPerformance(monsters);
}
