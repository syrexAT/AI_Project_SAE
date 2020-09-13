using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mit generics wie wir gelernt haben
//kein Monobehaviour weil nested state machines
//beeinhaltet States und Transitions um von a nach b zu kommen
//und eine referenz zum aktuellen state wo man ist

public class State<T> where T : MonoBehaviour
{
    public T objectReference;

    //3 funktionen die man überschreiben kann (virtual)
    public virtual void Update() { }
    public virtual void Entered() { }
    public virtual void Exited() { }

}

//State class but for SubStateMachine (Nested Statemachine; eine state die selber eine state beeinhaltet)
public class SubStateMachineState<T> : State<T> where T : MonoBehaviour
{
    public StateMachine<T> subStateMachine;
    //SubStateMachine entweder: wenn sie verlassen wird, wird die aktuelle State pausiert (es bleibt in dem state), wenn er wieder geentered wird läuft es von dort weiter
    //ODER wenn er den State verlässt, resettet sich die SubStateMachine KOMPLETT wieder (wie bei der folie mit dem head to trash usw. es startet am anfang des cycles wieder)

    public bool rememberStateWhenReentered; //um sich zu merken bei welchen state er rausgegangen ist

    public override void Update()
    {
        subStateMachine?.Update(); //? für nullCheck
    }
    public override void Entered()
    {

    }

    public override void Exited()
    {
        if (rememberStateWhenReentered)
        {
            subStateMachine?.Reset();
        }
    }

}

//man könnte hier reinschreiben von welchen State er zu welchen State geht (Transitions), das wird aber im Struct TransitionData geregelt
public abstract class Transition<T> where T : MonoBehaviour
{
    public T objectReference;

    //1 funktion, die überschreibbar sein muss!
    //es dürfen nicht 2 transitions gleichzeitig gestartet werden, es muss getrennt sein
    public abstract bool GetIsAllowed(); //abstract MUSS implementiert werden
}

public struct TransitionData<T> where T : MonoBehaviour
{
    public Transition<T> transition;
    public string from; //string im dictionary is der State
    public string to;
}

public class StateMachine<T> where T : MonoBehaviour
{
    //String name wie der State heisst, den state selbst
    public Dictionary<string, State<T>> states = new Dictionary<string, State<T>>(); //Dictionary mit allen States
    public List<TransitionData<T>> transitions = new List<TransitionData<T>>(); //Liste mit allen transitions
    public string initialState; //wird immer gebraucht, der anfangsstate

    //aktueller State wo man gerade ist, kan man als string oder object reference abspeichern, wir machens mit reference
    public State<T> currentState;

    //Liste an allen Transitions die momentan möglich sind!, man kümmert sich nur um die transition die von meiner State momentan weggehen!
    public List<TransitionData<T>> currentTransitions = new List<TransitionData<T>>();

    public void Update()
    {
        if (currentState == null)
        {
            //Man holt sich aus der transition jetzut die State aus dem Dictionary
            //man sucht zuerst wohin, dann reinspeichern in die variable
            //der key, und OUT parameter in welche variable es reingespeichert werden soll wenns gefunden wurde, 
            //es returned ein BOOL ob er einen Wert gefunden hat oder nicht
            if (states.TryGetValue(initialState, out currentState)) //TryGetValue initialState und speicher ihn in currentState --> Wenn das möglich ist gibt es TRUE zurück
            {
                currentState.Entered();

                //currentTransitions wieder holen, weil ejtzt sind noch die von der alten State
                currentTransitions.Clear();
                foreach (var transition in transitions)
                {
                    if (transition.from == initialState) //das heisst die transition ist möglich 
                    {
                        currentTransitions.Add(transition); //alle möglichen transitions adden die von der state weggehen
                    }
                }
            }
        }
        //Search for possible transitions
        //Alle möglichen Transitions durchgehen, wenn eine erfüllt ist, muss man auf der aktuellen aufrufen das man leftet
        //Und die neue State setzen und da Enter ausführen
        //Dann die Liste Clearen, alle Transitions aus der Liste suchen und wieder in die Liste auffüllen
        //damit man weiss was man als nächstes durchsuchen muss
        foreach (var currentTransition in currentTransitions)
        {
            if (currentTransition.transition.GetIsAllowed())
            {
                State<T> lastState = currentState; //cachen?

                if (states.TryGetValue(currentTransition.to, out currentState)) //Der State der rauskommt wenn die currentTransition zu einer state geht, wird in currenState gespeichert
                {
                    lastState.Exited(); //vorherigen State exiten
                    currentState.Entered(); //den neuen currentState enteren

                    //currentTransitions wieder holen weil jetzt sind noch die von der alten State obviously
                    currentTransitions.Clear();
                    foreach (var transition in transitions)
                    {
                        if (transition.from == currentTransition.to) //das heisst die transition ist möglich
                        {
                            currentTransitions.Add(transition);
                        }
                    }

                    return; //da sind wir fertig mim suchen
                }
            }
        }

        currentState?.Update(); //man nimmt sich die currentState und ruft da das Update auf

    }

    public void Reset()
    {
        //bei einem State immer wenn er irgendwie verlassen wird muss Exited(); aufgerufen werden (auch bei Reset von SubStateMachines)
        currentState?.Exited();
        currentState = null;
        currentTransitions.Clear();
    }

    public void AddState(State<T> state, string name)
    {
        states.Add(name, state);
    }

    public void AddTransition(Transition<T> transition, string from, string to)
    {
        //Geschwungene Klammer um nicht extra Constructor erstellen zu müssen
        transitions.Add(new TransitionData<T>() { transition = transition, from = from, to = to }); //variable naming is questionable
    }

    public void SetInitialState(string name) //ID mit der wir den State finden können
    {
        initialState = name;
    }


}


