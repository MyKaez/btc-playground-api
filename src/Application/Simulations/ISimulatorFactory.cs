namespace Application.Simulations;

public interface ISimulatorFactory
{
    ISimulator Create(string simulationType);
}