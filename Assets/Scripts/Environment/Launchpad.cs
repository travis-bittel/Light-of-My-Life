using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launchpad : MonoBehaviour
{
    [SerializeField]
    private Vector2 force;

    [SerializeField]
    private Vector2 setInitialVelocity;

    // Applies the force over the given period if true
    [SerializeField]
    private bool applyContinuously;
    [SerializeField]
    private float duration;

    // Changes the player's gravity during the duration
    [SerializeField]
    private float gravityScale;

    [SerializeField]
    private float playerManualMovementMultiplier;

    [SerializeField]
    private float cooldown;

    [SerializeField]
    private bool isReady;

    [SerializeField]
    public float maxHorizSpeed;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReady && collision.CompareTag("Player"))
        {
            StartCoroutine(Launch());
        }
    }

    // This code sucks but I don't have time to make it suck less
    private IEnumerator Launch()
    {
        Player.Instance.dashReady = true;
        Player.Instance.launchpadWithAuthority = this;
        Player.Instance.allowExceedingMaxHorizVelocity = true;
        Player.Instance.rb.velocity = setInitialVelocity;
        Player.Instance.additionalHorizAccelerationModifers.Add(playerManualMovementMultiplier);
        if (!applyContinuously && Player.Instance.launchpadWithAuthority == this)
        {
            Player.Instance.rb.AddForce(force);
        }
        isReady = false;
        Player.Instance.rb.gravityScale = gravityScale;
        float timePassed = 0;
        while (timePassed < cooldown || timePassed < duration)
        {
            timePassed += Time.deltaTime;
            if (timePassed < duration)
            {
                if (Player.Instance.launchpadWithAuthority != this)
                {
                    Player.Instance.additionalHorizAccelerationModifers.Remove(playerManualMovementMultiplier);
                    break;
                }
                if (applyContinuously && Player.Instance.launchpadWithAuthority == this)
                {
                    Player.Instance.rb.AddForce(Time.deltaTime * force);
                    if (maxHorizSpeed > 0)
                    {
                        Player.Instance.rb.velocity = new Vector2(Mathf.Clamp(Player.Instance.rb.velocity.x, -maxHorizSpeed, maxHorizSpeed), Player.Instance.rb.velocity.y);
                    }
                }
            } else
            {
                if (Player.Instance.launchpadWithAuthority == this)
                {
                    Player.Instance.rb.gravityScale = 1;
                    Player.Instance.allowExceedingMaxHorizVelocity = false;
                    Player.Instance.additionalHorizAccelerationModifers.Remove(playerManualMovementMultiplier);
                }
            }
            yield return null;
        }
        isReady = true;
    }
}
