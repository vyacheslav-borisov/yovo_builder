using System.Collections;
using UnityEngine;

public class MainMenuScene : Scene
{
    public void OnButtonClick_Play()
    {
        GameFlowManager.Instance.GoNextScene(SceneId.BRIEFING);                        
    }    

    public void OnButtonClick_Exit()
    {
        GameFlowManager.Instance.QuitGame();       
    }

    public void OnButtonClick_Yovo()
    {
        //TODO: add a beautyful pony magic here
    }

    public void OnButtonClick_Rate()
    {
        //TODO: add a beautyful pony magic here
    }
}
