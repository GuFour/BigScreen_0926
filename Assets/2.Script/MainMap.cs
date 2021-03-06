using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gu4.Extend;
using DG.Tweening;
using Gu4.Frame;

//==============================
//Synopsis  :  界面主地图
//CreatTime :  2021/10/14 9:42:14
//For       :  Gu4
//==============================

public class MainMap : MonoBehaviour
{
    #region 公开字段

    public Transform chinaMap { get { return transform.Find("中国地图"); } }
    public Transform hebeiMap { get { return chinaMap.Find("河北地图"); } }
    public Transform southMap { get { return hebeiMap.Find("南网地图"); } }
    public Transform sjzMap { get { return southMap.Find("SJZMap"); } }

    //城市标记点
    public Transform[] Points_City { get; private set; }

    //区域标记点
    public Transform[] Points_District { get; private set; }

    public Transform[] mapPoint2 { get; private set; }

    #endregion 公开字段

    //扩散的光环
    private SpriteRenderer burstDount;

    //河北地图描边序列帧
    private Animator heBeiEdge;

    //南网地图描边序列帧
    private Animator southEdge;

    //相机引用
    private Transform cam;

    //平行光源
    private Light directionalLight;

    //点光源
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
    /// 播放地图浮起动画
    /// </summary>
    public void PLayStartAnim()
    {
        //StartCoroutine(FloatMapAnim());
        StartCoroutine(FloatMapSetNow());
    }

    /// <summary>
    /// 初始化模型
    /// </summary>
    public void InitModel()
    {
        transform.SetPositionAndRotation(new Vector3(0f, -2.7f, 46.5f), Quaternion.Euler(0f, 0f, 0f));
        hebeiMap.localPosition = new Vector3(-12.14f, -0.55f, -4.58f);
        hebeiMap.TryGet<MeshRenderer>().enabled = false;
        southMap.localPosition = new Vector3(1.23f, 0.57f, 1.21f);
        sjzMap.TryGet<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        burstDount.transform.localScale = Vector3.zero;

        //将河北城市名调透明
        for (int i = 0; i < hebeiMap.Find("CityPoints").childCount; i++)
        {
            hebeiMap.Find("CityPoints").GetChild(i).
                TryGet<TextMesh>("New Text").color = Color.clear;
        }

        //将南网城市名调透明
        for (int i = 0; i < southMap.Find("SouthPoints").childCount; i++)
        {
            southMap.Find("SouthPoints").GetChild(i).
                TryGet<TextMesh>("New Text").color = Color.clear;
        }
    }

    /// <summary>
    /// 地图浮起动画
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
    /// 城市标记点显隐——连接对象池
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

    #region 弃用

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
    /// 区县标记点显隐——连接对象池
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
    /// 石家庄地图入场事件
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
                points[i].SetParent(sjzMap.Find("DistrictsPoints").GetChild(i)/*.Find("中国地图/河北地图/南网地图/SJZMap")*/);
                points[i].localPosition = Vector3.zero;
            }
        });
    }

    /// <summary>
    /// 石家庄地图出场事件
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

    #endregion 弃用
}