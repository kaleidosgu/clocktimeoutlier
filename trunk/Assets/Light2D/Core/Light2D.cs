using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LightEventListenerType { OnEnter, OnStay, OnExit }
public delegate void Light2DEvent(Light2D lightObject, GameObject objectInLight);

[ExecuteInEditMode()]
public class Light2D : MonoBehaviour
{
    public enum LightDetailSetting
    {
        VeryLow     =   74,
        Low         =   249,
        Normal      =   499,
        Medium      =   749,
        High        =   999,
        VeryHigh    =   1999,
        Extreme     =   3999
    }

    private static event Light2DEvent OnBeamEnter = null;
    private static event Light2DEvent OnBeamStay = null;
    private static event Light2DEvent OnBeamExit = null;

    private static int totalLightsRendered = 0;
    public static int TotalLightsRendered { get { return totalLightsRendered; } }
    private static int totalLightsUpdated = 0;
    public static int TotalLightsUpdated { get { return totalLightsUpdated; } }

    public bool ignoreOptimizations = false;
    public bool useEvents = false;
    public bool allowHideInsideColliders = false;

    [HideInInspector()]
    [SerializeField]
    private float lightRadius = 25;
    public float LightRadius { get { return lightRadius; } set { lightRadius = value; updateCircleLookup = true; } }

    [HideInInspector()]
    [SerializeField]
    private float sweepStart = 0;
    public float SweepStart { get { return sweepStart; } set { sweepStart = value; updateCircleLookup = true; } }

    [HideInInspector()]
    [SerializeField]
    private int sweepSize = 360;
    public int SweepSize { get { return sweepSize; } set { sweepSize = value; updateCircleLookup = true; } }

    [HideInInspector()]
    [SerializeField]
    private LightDetailSetting lightDetail = LightDetailSetting.Normal;
    public LightDetailSetting LightDetail { get { return lightDetail; } set { lightDetail = value; updateCircleLookup = true; updateNorms = true; } }

    [HideInInspector()]
    [SerializeField]
    private Color lightColor = Color.white;
    public Color LightColor { get { return lightColor; } set { lightColor = value; updateColor = true; } }

    [HideInInspector()]
    [SerializeField]
    private Material lightMaterial = null;
    public Material LightMaterial { get { return lightMaterial; } set { lightMaterial = value; } }

    [HideInInspector()]
    [SerializeField]
    private bool lightStatic = false;
    public bool IsStatic { get { return lightStatic; } set { lightStatic = value; } }

    public bool IsVisible { get { if (renderer) return renderer.isVisible; else return false; } }

    public LayerMask ShadowLayer;

    private bool updateMesh = true;
    private bool updateColor = true;
    private bool updateCircleLookup = true;
    private bool updateNorms = true;
    private bool initialized = false;
    private bool insideObject = false;

    private Vector3[] circleLookUp;

    private Mesh _mesh = null;
    private MeshFilter _filter = null;
    
    private Collider[] potentialShadowObject;
    private List<GameObject> knownObjects = new List<GameObject>(512);
    private List<GameObject> unknownObjects = new List<GameObject>(512);

    private Color[] mColors = null;
    private List<int> tris = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<Vector3> verts = new List<Vector3>();

    private List<Vector3> curColliderPositions = new List<Vector3>();
    private List<Vector3> preColliderPositions = new List<Vector3>();
    private Vector3 myPrevPosition = new Vector3(3249, 3495, -2384);

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "Light.png", true);
    }

    void InitializeLight()
    {
        if (_mesh == null)
            _mesh = new Mesh();
        _mesh.name = "LightMesh_" + gameObject.GetInstanceID();

        if (!lightMaterial)
            lightMaterial = (Material)Resources.Load("RadialLightMaterial", typeof(Material));

        if (!(_filter = gameObject.GetComponent<MeshFilter>()))
            _filter = gameObject.AddComponent<MeshFilter>();

        _filter.hideFlags = HideFlags.HideInInspector;

        if (!gameObject.renderer)
            gameObject.AddComponent<MeshRenderer>();

        gameObject.renderer.hideFlags = HideFlags.HideInInspector;
        ShadowLayer = 1;

        UpdateCircleLookup();
        UpdateLightMesh();

        initialized = true;
    }

    void Update()
    {
        totalLightsRendered = 0;
        totalLightsUpdated = 0;
    }
    void LateUpdate()
    {
        UpdateLight2D();
    }

    public void UpdateLight2D()
    {
        Vector3 pos = transform.position;
        if (HasMoved(ref myPrevPosition, pos))
            updateMesh = true;

        potentialShadowObject = Physics.OverlapSphere(pos, lightRadius / 2, ShadowLayer);
        curColliderPositions.Clear();
        foreach (Collider c in potentialShadowObject)
        {
            if (c == gameObject.collider)
                continue;

            if (allowHideInsideColliders)
            {
                curColliderPositions.Add(c.transform.position);

                if (c.bounds.Contains(pos))
                {
                    insideObject = true;
                    break;
                }
            }
            else
            {
                curColliderPositions.Add(c.transform.position);
            }
        }

        if (!updateMesh)
        {
            if (curColliderPositions.Count != preColliderPositions.Count)
            {
                updateMesh = true;
            }
            else
            {
                for (int i = 0; i < curColliderPositions.Count; i++)
                {
                    Vector3 prePos = preColliderPositions[i];
                    if (HasMoved(ref prePos, curColliderPositions[i]))
                    {
                        updateMesh = true;
                        break;
                    }
                }
            }
        }
        preColliderPositions.Clear();
        preColliderPositions.AddRange(curColliderPositions);

        if (!initialized)
            InitializeLight();

        if (updateCircleLookup)
            UpdateCircleLookup();

        if (!Application.isPlaying)
        {
            UpdateLightMesh();

            if (updateColor)
                UpdateLightColor();
        }
        else if(renderer.isVisible)
        {
            totalLightsRendered++;

            if ((updateMesh || ignoreOptimizations) && !IsStatic)
                UpdateLightMesh();

            if (updateColor)
                UpdateLightColor();
        }
    }

    static bool HasMoved(ref Vector3 prevPoint, Vector3 curPoint)
    {
        bool r = Vector3.Distance(curPoint, prevPoint) >= 0.01f;

        if (r) prevPoint = curPoint;

        return r;
    }

    void UpdateCircleLookup()
    {
        float ld = ((float)sweepSize / (int)lightDetail);
        circleLookUp = new Vector3[(int)lightDetail + 1];
        for (int i = 0; i < circleLookUp.Length; i++)
            circleLookUp[i] = new Vector3((Mathf.Cos((sweepStart + (ld * (i + 1))) * Mathf.Deg2Rad)), (Mathf.Sin((sweepStart + (ld * (i + 1))) * Mathf.Deg2Rad)), 0) / 2f;

        circleLookUp[circleLookUp.Length-1] = new Vector3((Mathf.Cos((sweepStart) * Mathf.Deg2Rad)), (Mathf.Sin((sweepStart) * Mathf.Deg2Rad)), 0) / 2f;

        updateCircleLookup = false;
    }

    void UpdateNormals(Mesh m)
    {
        Vector3[] norms = new Vector3[m.vertexCount];

        for (int i = 0; i < m.vertexCount; i++)
            norms[i] = -transform.forward;

        m.normals = norms;

        updateNorms = false;
    }

    void UpdateLightMesh()
    {
        totalLightsUpdated++;

        _mesh.Clear();
        verts.Clear();
        tris.Clear();
        tris.TrimExcess();
        uvs.Clear();

        if (insideObject)
        {
            insideObject = false;
            return;
        }
        Vector3 v1, v2, pv = Vector3.zero;
        Vector3 pos = transform.position;
        RaycastHit rh;

        if (useEvents && Application.isPlaying)
            unknownObjects.Clear();

        for (int i = 0; i < (int)lightDetail; i++)
        {
            verts.Add(Vector3.zero);
            uvs.Add(new Vector2(0.5f, 0.5f));

            if (i == 0)
            {
                //v1 = new Vector3(lightRadius * (Mathf.Cos((sweepStart + (ld * i)) * Mathf.Deg2Rad)), lightRadius * (Mathf.Sin((sweepStart + (ld * i)) * Mathf.Deg2Rad)), 0) / 2;
                v1 = circleLookUp[circleLookUp.Length - 1] * lightRadius;
                if (Physics.Raycast(pos, transform.TransformDirection(v1), out rh, lightRadius / 2, ShadowLayer))
                {
                    v1 = transform.InverseTransformPoint(rh.point);

                    if (useEvents && Application.isPlaying && !unknownObjects.Contains(rh.collider.gameObject))
                        unknownObjects.Add(rh.collider.gameObject);
                }

                verts.Add(v1);
                uvs.Add(new Vector2((0.5f + (v1.x) / lightRadius), (0.5f + (v1.y) / lightRadius)));
            }
            else
            {
                verts.Add(pv);
                uvs.Add(new Vector2((0.5f + (pv.x) / lightRadius), (0.5f + (pv.y) / lightRadius)));
            }


            //v2 = pv = new Vector3(lightRadius * (Mathf.Cos((sweepStart + (ld * (i + 1))) * Mathf.Deg2Rad)), lightRadius * (Mathf.Sin((sweepStart + (ld * (i + 1))) * Mathf.Deg2Rad)), 0) / 2;
            v2 = pv = circleLookUp[i] * lightRadius;
            if (Physics.Raycast(pos, transform.TransformDirection(v2), out rh, lightRadius / 2, ShadowLayer))
            {
                v2 = pv = transform.InverseTransformPoint(rh.point);

                if (useEvents && Application.isPlaying && !unknownObjects.Contains(rh.collider.gameObject))
                    unknownObjects.Add(rh.collider.gameObject);
            }

            verts.Add(v2);

            uvs.Add(new Vector2((0.5f + (v2.x) / lightRadius), (0.5f + (v2.y) / lightRadius)));

            tris.Add((i * 3) + 2); tris.Add((i * 3) + 1); tris.Add((i * 3) + 0);

            //norms.Add(-transform.up); norms.Add(-transform.up); norms.Add(-transform.up);
        }

        if(useEvents && Application.isPlaying)
            DelegateEvents();

        _mesh.vertices = verts.ToArray();
        _mesh.uv = uvs.ToArray();
        //_mesh.uv2 = uvs.ToArray();
        //_mesh.normals = norms.ToArray();
        _mesh.triangles = tris.ToArray();
        _mesh.RecalculateBounds();

        //if (updateUVs)
        //    UpdateUVs(_mesh);
        if (updateNorms)
            UpdateNormals(_mesh);

        _filter.mesh = _mesh;

        renderer.sharedMaterial = lightMaterial;
        
        UpdateLightColor();
        updateMesh = false;
    }

    void DelegateEvents()
    {
        for (int i = 0; i < unknownObjects.Count; i++)
        {
            if (knownObjects.Contains(unknownObjects[i]))
            {
                if (OnBeamStay != null)
                    OnBeamStay(this, unknownObjects[i]);
            }
            else
            {
                knownObjects.Add(unknownObjects[i]);

                if (OnBeamEnter != null)
                    OnBeamEnter(this, unknownObjects[i]);
            }
        }

        for (int i = 0; i < knownObjects.Count; i++)
        {
            if (!unknownObjects.Contains(knownObjects[i]))
            {
                if (OnBeamExit != null)
                    OnBeamExit(this, knownObjects[i]);

                knownObjects.Remove(knownObjects[i]);
            }
        }
    }

    public static void RegisterEventListener(LightEventListenerType eventType, Light2DEvent eventMethod)
    {
        if (eventType == LightEventListenerType.OnEnter)
            OnBeamEnter += eventMethod;

        if (eventType == LightEventListenerType.OnStay)
            OnBeamStay += eventMethod;

        if (eventType == LightEventListenerType.OnExit)
            OnBeamExit += eventMethod;
    }
    public static void UnregisterEventListener(LightEventListenerType eventType, Light2DEvent eventMethod)
    {
        if (eventType == LightEventListenerType.OnEnter)
            OnBeamEnter -= eventMethod;

        if (eventType == LightEventListenerType.OnStay)
            OnBeamStay -= eventMethod;

        if (eventType == LightEventListenerType.OnExit)
            OnBeamExit -= eventMethod;
    }

    void UpdateLightColor()
    {
        if (updateColor)
        {
            Mesh m = _filter.sharedMesh;
            mColors = new Color[m.vertexCount];
            for (int c = 0; c < mColors.Length; c++)
            {
                mColors[c] = lightColor;
            }
            m.colors = mColors;

            updateColor = false;
        }
        else
        {
            _filter.sharedMesh.colors = mColors;
        }
    }

    void OnEnable()
    {
        _mesh = null;
        _filter = null;

        updateColor = true;
        updateMesh = true;
        updateCircleLookup = true;
        initialized = false;
    }

    public static Light2D Create(Vector3 position, Material lightMaterial, Color lightColor)
    {
        return Create(position, lightMaterial, lightColor, 10, 0, 360, LightDetailSetting.Normal);
    }

    public static Light2D Create(Vector3 position, Material lightMaterial, Color lightColor, float lightRadius)
    {
        return Create(position, lightMaterial, lightColor, lightRadius, 0, 360, LightDetailSetting.Normal);
    }

    public static Light2D Create(Vector3 position, Material lightMaterial, Color lightColor, float lightRadius, float sweepStart, int sweepSize)
    {
        return Create(position, lightMaterial, lightColor, lightRadius, sweepStart, sweepSize, LightDetailSetting.Normal);
    }

    public static Light2D Create(Vector3 position, Material lightMaterial, Color lightColor, float lightRadius, float sweepStart, int sweepSize, LightDetailSetting detailSetting)
    {
        GameObject go = new GameObject("Created Light");
        go.transform.position = position;
        
        Light2D nLight = go.AddComponent<Light2D>();
        nLight.lightMaterial = lightMaterial;
        nLight.lightColor = lightColor;
        nLight.lightRadius = lightRadius;
        nLight.sweepStart = sweepStart;
        nLight.sweepSize = sweepSize;
        nLight.lightDetail = detailSetting;

        return nLight;
    }
}
