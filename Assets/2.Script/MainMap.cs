using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gu4.Extend;
using DG.Tweening;
using Gu4.Frame;

//==============================
//Synopsis  :  ��������ͼ
//CreatTime :  2021/10/14 9:42:14
//For       :  Gu4
//==============================

public class MainMap : MonoBehaviour
{
    #region �����ֶ�

    public Transform chinaMap { get { return transform.Find("�й���ͼ"); } }
    public Transform hebeiMap { get { return chinaMap.Find("�ӱ���ͼ"); } }
    public Transform southMap { get { return hebeiMap.Find("������ͼ"); } }
    public Transform sjzMap { get { return southMap.Find("SJZMap"); } }

    //���б�ǵ�
    public Transform[] Points_City { get; private set; }

    //�����ǵ�
    public Transform[] Points_District { get; private set; }

    public Transform[] mapPoint2 { get; private set; }

    #endregion �����ֶ�

    //��ɢ�Ĺ⻷
    private SpriteRenderer burstDount;

    //�ӱ���ͼ�������֡
    private Animator heBeiEdge;

    //������ͼ�������֡
    private Animator southEdge;

    //�������
    private Transform cam;

    //ƽ�й�Դ
    private Light directionalLight;

    //���Դ
    private Light spotLight;

    private void Awake()
    {
        burstDount = southMap.TryGet<SpriteRenderer>("BurstDount");
        heBeiEdge = hebeiMap.TryGet<Animator>("HeBeiEdge");
        southEdge = hebeiMap.TryGet<Animator>("SouthEdge");

        cam = Camera.main.transform;
        directionalLight = GameObject.Find("Directional Light").transform.TryGet<Light>();
        spotLight = GameObject.Find("Point Light").transform.TryGet<Light>();

        Points_City = new Transform[7];
        mapPoint2 = new Transform[6];
        Points_District = new Transform[4];

        InitModel();
    }

    /// <summary>
    /// ���ŵ�ͼ���𶯻�
    /// </summary>
    public void PLayStartAnim()
    {
        //StartCoroutine(FloatMapAnim());
        StartCoroutine(FloatMapSetNow());
    }

    /// <summary>
    /// ��ʼ��ģ��
    /// </summary>
    public void InitModel()
    {
        transform.SetPositionAndRotation(new Vector3(0f, -2.7f, 46.5f), Quaternion.Euler(0f, 0f, 0f));
        hebeiMap.localPosition = new Vector3(-12.14f, -0.55f, -4.58f);
        hebeiMap.TryGet<MeshRenderer>().enabled = false;
        southMap.localPosition = new Vector3(1.23f, 0.57f, 1.21f);
        sjzMap.TryGet<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        burstDount.transform.localScale = Vector3.zero;

        //���ӱ���������͸��
        for (int i = 0; i < hebeiMap.Find("CityPoints").childCount; i++)
        {
            hebeiMap.Find("CityPoints").GetChild(i).
                TryGet<TextMesh>("New Text").color = Color.clear;
        }

        //��������������͸��
        for (int i = 0; i < southMap.Find("SouthPoints").childCount; i++)
        {
            southMap.Find("SouthPoints").GetChild(i).
                TryGet<TextMesh>("New Text").color = Color.clear;
        }
    }

    /// <summary>
    /// ��ͼ���𶯻�
    /// </summary>
    /// <returns></returns>
    private IEnumerator FloatMapAnim()
    {
        cam.SetPositionAndRotation(new Vector3(0f, 45f, 49f), Quaternion.Euler(90f, 0f, 0f));

        yield return new WaitForSeconds(1f);

        cam.DOMove(new Vector3(10.64f, 0.3800004f, 45.08987f)/*new Vector3(10.64f, 0.96f, 45.37f)*/, 2f);
        cam.DORotate(new Vector3(30f, 0f, 0f), 2f);

        transform.DOScaleX(1.2f, 2f);
        transform.DOMove(new Vector3(-2.2f, -2.7f, 46.5f), 2f);

        yield return new WaitForSeconds(2f);

        directionalLight.DOIntensity(.5f, 4f);
        southEdge.SetBool("IsPlay", true);

        yield return new WaitForSeconds(4f);

        spotLight.DOIntensity(12f, 4f);

        southEdge.transform.TryGet<SpriteRenderer>().
            DOFade(0f, 2f).SetEase(Ease.Linear);
        hebeiMap.DOLocalMoveY(0.42f, 2f);
        southMap.DOLocalMoveY(0.75f, 2f).OnComplete(() =>
        {
            for (int i = 0; i < hebeiMap.Find("CityPoints").childCount; i++)
            {
                hebeiMap.Find("CityPoints").GetChild(i).
                    TryGet<TextMesh>("New Text").color = Color.gray;
            }

            for (int i = 0; i < southMap.Find("SouthPoints").childCount; i++)
            {
                southMap.Find("SouthPoints").GetChild(i).
                    TryGet<TextMesh>("New Text").color = Color.white;
            }
        });

        southMap.TryGet<HighlightableObject>().ConstantOn(
         new Color(0.01960784f, 0.4117647f, 1f, 1f));

        yield return new WaitForSeconds(60f);

        burstDount.transform.localScale = Vector3.zero;
        burstDount.color = new Color(1f, 1f, 1f, 1f);
        burstDount.transform.DOScale(Vector3.one * 5f, 2f);
        burstDount.DOFade(0f, 3f);

        yield return new WaitForSeconds(2f);

        Get_Point_City(true);
        Points_City[0].TryGet<Point_City>().PlayNumberAnim(35, "", 0f);
        Points_City[1].TryGet<Point_City>().PlayNumberAnim(36, "", 0f);
        Points_City[2].TryGet<Point_City>().PlayNumberAnim(21, "", 0f);
        Points_City[3].TryGet<Point_City>().PlayNumberAnim(57, "", 0f);
        Points_City[4].TryGet<Point_City>().PlayNumberAnim(10, "", 0f);
        Points_City[5].TryGet<Point_City>().PlayNumberAnim(6, "", 0f);
        Points_City[6].TryGet<Point_City>().PlayNumberAnim(36, "", 0f);

        GameManager.m_Instance.gameProgress = 0;
    }

    public IEnumerator FloatMapSetNow()
    {
        cam.SetPositionAndRotation(new Vector3(10.64f, 0.3800004f, 45.08987f),
            Quaternion.Euler(new Vector3(30f, 0f, 0f)));

        directionalLight.intensity = .5f;

        spotLight.intensity = 12f;

        transform.position = new Vector3(-2.2f, -2.7f, 46.5f);
        transform.localScale = new Vector3(1.2f, 1f, 1f);

        hebeiMap.localPosition = new Vector3(hebeiMap.localPosition.x, .42f, hebeiMap.localPosition.z);
        southMap.localPosition = new Vector3(southMap.localPosition.x, .75f, southMap.localPosition.z);
        for (int i = 0; i < hebeiMap.Find("CityPoints").childCount; i++)
        {
            hebeiMap.Find("CityPoints").GetChild(i).
               TryGet<TextMesh>("New Text").color = Color.gray;
        }

        for (int i = 0; i < southMap.Find("SouthPoints").childCount; i++)
        {
            southMap.Find("SouthPoints").GetChild(i).
               TryGet<TextMesh>("New Text").color = Color.white;
        }

        southMap.Find("Model").TryGet<HighlightableObject>().ConstantOn(
       new Color(0.01960784f, 0.4117647f, 1f, 1f));

        yield return new WaitForSeconds(.5f);

        burstDount.transform.localScale = Vector3.zero;
        burstDount.color = new Color(1f, 1f, 1f, 1f);
        burstDount.transform.DOScale(Vector3.one * 5f, 2f);
        burstDount.DOFade(0f, 3f);

        Get_Point_City(true);

        Points_City[0].TryGet<Point_City>().PlayNumberAnim(35, "", 0f);
        Points_City[1].TryGet<Point_City>().PlayNumberAnim(36, "", 0f);
        Points_City[2].TryGet<Point_City>().PlayNumberAnim(21, "", 0f);
        Points_City[3].TryGet<Point_City>().PlayNumberAnim(57, "", 0f);
        Points_City[4].TryGet<Point_City>().PlayNumberAnim(10, "", 0f);
        Points_City[5].TryGet<Point_City>().PlayNumberAnim(6, "", 0f);
        Points_City[6].TryGet<Point_City>().PlayNumberAnim(36, "", 0f);

        GameManager.m_Instance.gameProgress = 0;
        cam.gameObject.AddComponent<CameraCtrl>();
    }

    /// <summary>
    /// ���б�ǵ������������Ӷ����
    /// </summary>
    /// <param name="isShow"></param>
    /// <returns></returns>
    public Transform[] Get_Point_City(bool isShow)
    {
        for (int i = 0; i < Points_City.Length; i++)
        {
            if (isShow)
            {
                Points_City[i] = ObjectPools.m_Instance.GetObject("Model/Point_City").transform;
                Points_City[i].SetParent(southMap.Find("SouthPoints").GetChild(i));
                Points_City[i].localPosition = Vector3.zero;
                Points_City[i].localEulerAngles = Vector3.zero;
            }
            else
            {
                ObjectPools.m_Instance.RecycleObject(Points_City[i].gameObject);
            }
        }

        return Points_City;
    }

    #region ����

    public void MapPoint2Ctrl(bool isOpen)
    {
        for (int i = 0; i < mapPoint2.Length; i++)
        {
            if (isOpen)
            {
                mapPoint2[i] = ObjectPools.m_Instance.GetObject("Model/MapPoint2").transform;
                mapPoint2[i].DOScale(1f, 1f);
            }
            else
            {
                //mapPoint2s[i].DOScale(0f, 1f);
                ObjectPools.m_Instance.RecycleObject(mapPoint2[i].gameObject);
            }
        }
    }

    /// <summary>
    /// ���ر�ǵ������������Ӷ����
    /// </summary>
    /// <param name="isShow"></param>
    /// <returns></returns>
    public Transform[] Get_Point_Districts(bool isShow)
    {
        for (int i = 0; i < Points_District.Length; i++)
        {
            if (isShow)
            {
                Points_District[i] = ObjectPools.m_Instance.GetObject("Model/Point_Districts").transform;
                //Points_District[i].DOScale(1f, 1f);
            }
            else
            {
                Points_District[i].DOScale(0f, 1f).OnComplete(() =>
                {
                    ObjectPools.m_Instance.RecycleObject(Points_District[i].gameObject);
                });
            }
        }

        return Points_District;
    }

    /// <summary>
    /// ʯ��ׯ��ͼ�볡�¼�
    /// </summary>
    /// <param name="dura"></param>
    public void SJZMapEnter(float dura = 2f)
    {
        cam.DOMove(new Vector3(9.94f, -1.47f, 48.66f), 2f);
        cam.DORotate(new Vector3(45f, 0f, 0f), 2f);

        sjzMap.TryGet<SpriteRenderer>().DOFade(1f, dura).OnComplete(() =>
        {
            Transform[] points = GameManager.m_Instance.mainMap.Get_Point_Districts(true);
            for (int i = 0; i < points.Length; i++)
            {
                points[i].SetParent(sjzMap.Find("DistrictsPoints").GetChild(i)/*.Find("�й���ͼ/�ӱ���ͼ/������ͼ/SJZMap")*/);
                points[i].localPosition = Vector3.zero;
            }
        });
    }

    /// <summary>
    /// ʯ��ׯ��ͼ�����¼�
    /// </summary>
    /// <param name="dura"></param>
    public void SJZMapExit(float dura = .5f)
    {
        //points = GameManager.m_Instance.mainMap.Get_Point_Districts(false);
        //sprenderer.DOFade(0f, .5f);

        sjzMap.TryGet<SpriteRenderer>().DOFade(0f, dura);

        Point_Districts[] points_Districts = new Point_Districts[sjzMap.childCount];

        for (int i = 0; i < sjzMap.childCount; i++)
        {
            points_Districts[i] = sjzMap.Find("DistrictsPoints").GetChild(i).TryGet<Point_Districts>("Point_Districts");

            ObjectPools.m_Instance.RecycleObject(points_Districts[i].gameObject);
        }
    }

    #endregion ����
}