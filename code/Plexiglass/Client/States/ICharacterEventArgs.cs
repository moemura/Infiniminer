namespace Plexiglass.Client.States
{ 
    public interface ICharacterEventArgs
    {
        char Character { get; }
        int Param { get; }
        int RepeatCount { get; }
        bool ExtendedKey { get; }
        bool AltPressed { get; }
        bool PreviousState { get;}
        bool TransitionState { get; }
    }
}