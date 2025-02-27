using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


public class BallBehave : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = -0.15f;
    public Transform platforms;
    public Transform cameraPos;
    public Text score_text;
    public Text combo_text;
    public LayerMask plLayer;
    public LayerMask dhLayer;
    public LayerMask cnLayer;
    public float colR;
    public float maxSpeed = 20;
    public float dec;
    private float yvel;
    public GameObject handler;
    private float clkPos;
    private int last_plat = -20;
    private int combo = 1;
    public GameObject gameUI;
    public bool paused;
    public bool sideCollided = false;
    public bool allowCameraMove = true;
    public Animation anim, coinAnim;
    public Text moneyText;

    public Material platMat;
    public Material poleMat;
    public Camera cameraC;


    int get_pref(string field)
    {
        // �������� ����� �� player pref
        float value = Mathf.Pow(PlayerPrefs.GetFloat(field), 8.5f);
        if (Mathf.Abs(value - Mathf.RoundToInt(value)) < 0.001f)
        {
            return Mathf.RoundToInt(value);
        }
        else
        {
            return 0;
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Application.targetFrameRate = 60;
        rb.velocity = new Vector3(0, -10f, 0);

        coinAnim["moneyCollectAnim"].time = 0;
        coinAnim["moneyCollectAnim"].speed = 0;
        coinAnim.Play();

        platMat.color = UnityEngine.Color.HSVToRGB(200 / 360f, 0.9f, 0.9f);
        poleMat.color = UnityEngine.Color.HSVToRGB(190 / 360f, 0.75f, 0.5f);
        cameraC.backgroundColor = UnityEngine.Color.HSVToRGB(220 / 360f, 0.5f, 0.7f);
        rb.gameObject.GetComponent<SkinHandler>().Reload();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ScorePlus()
    {
        // ������� ���������� ��������
        score_text.text = (int.Parse(score_text.text) + 1 * combo).ToString();
        combo_text.text = "x" + (combo).ToString();
        combo_text.color = new UnityEngine.Color(Mathf.Min(0.196f + (combo - 1) * 0.25f, 1), 0.196f, 0.196f);
        if (combo > 2)
        {
            combo_text.gameObject.GetComponent<Animation>().enabled = true;
        }
        combo += 1;
        last_plat -= 10;
        allowCameraMove = true;

        platMat.color = UnityEngine.Color.HSVToRGB(Mathf.Clamp(200-int.Parse(score_text.text), 0, 360) /360f, 0.9f, 0.9f);
        poleMat.color = UnityEngine.Color.HSVToRGB(Mathf.Clamp(190 - int.Parse(score_text.text), 0, 360) / 360f, 0.75f, 0.5f);
        cameraC.backgroundColor = UnityEngine.Color.HSVToRGB(Mathf.Clamp(220 - int.Parse(score_text.text), 0, 360) / 360f, 0.5f, 0.7f);
    }


    public void Restart()
    {
        combo = 1;
        
        rb.velocity = new Vector3(0, 0, 0);
        float camOffset = rb.position.y - cameraPos.position.y;
        
        rb.position = new Vector3(0, rb.position.y - last_plat + Mathf.Max(-40, last_plat), -2.5f);
        cameraPos.position = new Vector3(cameraPos.position.x, cameraPos.position.y - last_plat + Mathf.Max(-40, last_plat), cameraPos.position.z);
        // Debug.Log("reset");
        last_plat = -20;

        platforms.gameObject.GetComponent<PlatBehave>().RestartPlat();
        cameraPos.gameObject.GetComponent<CameraBehave>().StartAnimaton(int.Parse(score_text.text));

    }
    public void Death()
    {
        gameUI.GetComponent<GameUIScript>().game_end();
    }


    private void FixedUpdate()
    {
        paused = gameUI.GetComponent<GameUIScript>().paused;

        if (!paused)
        {
            if (yvel != 0)
            {
                rb.velocity = new Vector3(0, yvel, 0);
                yvel = 0;
            }
            
            Vector3 gndVec = new Vector3(rb.position.x, rb.position.y - 0.25f, rb.position.z);
            Collider[] gnd = Physics.OverlapSphere(gndVec, colR-0.25f, plLayer);
            Vector3 deathVec = new Vector3(rb.position.x, rb.position.y-0.35f, rb.position.z);
            Collider[] death = Physics.OverlapSphere(deathVec, colR-0.35f, dhLayer);

            Collider[] coins = Physics.OverlapSphere(rb.position, 1, cnLayer);

            Collider[] sideCollider = Physics.OverlapSphere(rb.position, colR+0.1f, plLayer);


            speed = speed + dec;

            if (death.Length != 0) // �������� ������
            {
                Death();
            }

            // ���� �������
            if (coins.Length != 0)
            {;
                moneyText.text = "+$" + (coins[0].GetComponent<MoneyScript>().amount).ToString();
                coinAnim["moneyCollectAnim"].time = 0;
                coinAnim["moneyCollectAnim"].speed = 1;
                coinAnim.Play();
                PlayerPrefs.SetFloat("money", Mathf.Pow(get_pref("money") + coins[0].GetComponent<MoneyScript>().amount, 0.11764705f));
                coins[0].GetComponent<MoneyScript>().SetCollider(false);   
            }



            // ������� ���������
            if (gnd.Length != 0 && rb.velocity.y <= 0)
            {
                anim.Play();
                speed = 0;
                rb.velocity = new Vector3(rb.velocity.x, 7, rb.velocity.z);
                combo = 1;
                combo_text.text = "x" + (combo).ToString();
                combo_text.color = new Color(0.196f, 0.196f, 0.196f);
                combo_text.gameObject.GetComponent<Animation>().enabled = false;
                // rb.position = new Vector3(rb.position.x, Mathf.FloorToInt(rb.position.y / 10)*10 + 1.5f, rb.position.z);
            }

            // ������������ ��������
            if (rb.velocity.y + speed < maxSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, maxSpeed, rb.velocity.z);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + speed, rb.velocity.z);
            }


            // ������� �����
            if (rb.position.y < last_plat - 0.3f)
            {
                ScorePlus();
            }

            // ����������
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Input.mousePosition;
                if (clkPos == -1)
                {
                    clkPos = mousePos.x;
                }
                else
                {
                    if ((sideCollider.Length == 0 || gnd.Length != 0) && death.Length == 0)
                    {
                        platforms.eulerAngles = new Vector3(0, platforms.eulerAngles.y + (clkPos - mousePos.x) / 4, 0);
                        clkPos = mousePos.x;
                    } else
                    {
                        clkPos = mousePos.x;
                    }
                    
                }

            }
            else
            {
                clkPos = -1;
            }

            if ((cameraPos.position.y - rb.position.y > 4) && allowCameraMove) // ������
            {
                cameraPos.position = new Vector3(cameraPos.position.x, rb.position.y + 4, cameraPos.position.z);
            }
        } else
        {
            if (yvel == 0)
            {
                yvel = rb.velocity.y;
                rb.velocity = new Vector3(0, 0, 0);
            }
        }
        sideCollided = false;

    }

    /*private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("col");
    }*/
}
