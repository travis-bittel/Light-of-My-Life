using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// When open, players can interact with an unlocked gate to move to the next region.
public class Gate : MonoBehaviour
{
    private int unlockProgress;
    [SerializeField]
    private int progressRequiredForUnlock;

    public bool isUnlocked;

    public string nextScene;
    
    public void AdjustUnlockProgress(int value)
    {
        unlockProgress += value;
        if (unlockProgress >= progressRequiredForUnlock)
        {
            isUnlocked = true;
            // Change to open sprite
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player.Instance.gateWithinRange = this;
            if (isUnlocked)
            {
                TextManager.Instance.DisplayFloatingText("Press <b><i>E</b></i> to enter the Doorway", Color.white);
            } else
            {
                if (progressRequiredForUnlock - unlockProgress == 1)
                {
                    TextManager.Instance.DisplayFloatingText("The Doorway is sealed shut. Activate <b><i>"
                    + (progressRequiredForUnlock - unlockProgress)
                    + "</b></i> more Lantern to open it", Color.white);
                } else
                {
                    TextManager.Instance.DisplayFloatingText("The Doorway is sealed shut. Activate <b><i>"
                    + (progressRequiredForUnlock - unlockProgress)
                    + "</b></i> more Lanterns to open it", Color.white);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Player.Instance.gateWithinRange == this)
        {
            Player.Instance.gateWithinRange = null;
            TextManager.Instance.DisplayFloatingText("", Color.white);
        }
    }
}
