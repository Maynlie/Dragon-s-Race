using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update
    bool isTouching = false;
    bool isSwipping = false;
    Vector2 touchOriginPos;
    bool invincibleMode = false;
    float invincibleStart = 0.0f;
    float ZPos;
    public float invicibleTimeMax = 5.0f;
    public AudioSource mainMusic;
    public AudioSource speedMusic;
    public AudioSource effectSource;
    public AudioClip feedEffect;
    public AudioClip starEffect;
    public AudioClip bumpEffect;

    public Renderer rend;
    public Material normalMat;
    public Material speedMat;

    void Start()
    {
        Input.simulateMouseWithTouches = true;
        ZPos = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if(invincibleMode)
        {
            if(Time.time - invincibleStart >= invicibleTimeMax)
            {
                invincibleMode = false;
                rend.material = normalMat;
                speedMusic.Stop();
                speedMusic.volume = 0;
                mainMusic.volume = 0.5f;
                GroundGenerator ground = GameObject.Find("GroundGenerator").GetComponent<GroundGenerator>();
                ground.invincibleMode = false;
            }
        }

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if(t.phase == TouchPhase.Began)
            {
                isSwipping = true;
                touchOriginPos = t.position;
            }
            if(t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                isSwipping = false;
            }
            //Vector2 deltaPos = new Vector2(t.deltaPosition.x / 1000000000, 0);
            //slideSnake(deltaPos);
        }

        if(isSwipping && Input.touchCount > 0)
        {
            Vector2 currentPos = Input.touches[0].position;
            Vector2 deltaPos = new Vector2(currentPos.x - touchOriginPos.x, 0);
            slideSnake(deltaPos);

            touchOriginPos = currentPos;
        }

        if(Input.GetMouseButtonDown(0))
        {
            isTouching = true;
            touchOriginPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isTouching = false;
        }

        if(isTouching)
        {
            Vector2 currentPos = Input.mousePosition;
            Vector2 deltaPos = new Vector2(currentPos.x - touchOriginPos.x, 0);
            slideSnake(deltaPos);
           
            touchOriginPos = currentPos;
        }
        if(GetComponent<Transform>().position.y <= -2)
        {
            GroundGenerator ground = GameObject.Find("GroundGenerator").GetComponent<GroundGenerator>();
            ground.lostTheGame();
        }
    }

    void slideSnake(Vector2 deltaPos)
    {
        gameObject.transform.Translate(deltaPos * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (!invincibleMode)
            {
                GroundGenerator ground = GameObject.Find("GroundGenerator").GetComponent<GroundGenerator>();
                effectSource.PlayOneShot(bumpEffect);
                ground.lostTheGame();
            }
            else
            {
                other.gameObject.SetActive(false);
            }
        }
        if (other.gameObject.CompareTag("Star"))
        {
            GroundGenerator ground = GameObject.Find("GroundGenerator").GetComponent<GroundGenerator>();
            ground.invincibleMode = true;
            invincibleMode = true;
            rend.material = speedMat;
            effectSource.PlayOneShot(starEffect);
            if(!speedMusic.isPlaying)
            {
                speedMusic.volume = 1;
                speedMusic.Play();
                mainMusic.volume = 0;
            }
            other.gameObject.SetActive(false);
            invincibleStart = Time.time;
        }
        if (other.gameObject.CompareTag("Food"))
        {
            GroundGenerator ground = GameObject.Find("GroundGenerator").GetComponent<GroundGenerator>();
            ground.score++;
            effectSource.PlayOneShot(feedEffect);
            other.gameObject.SetActive(false);
        }
    }
}
