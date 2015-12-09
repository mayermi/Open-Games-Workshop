using UnityEngine;
using RSUnityToolkit;


public class HandTrackingAction : VirtualWorldBoxAction
{

    #region Public Fields

    public GameObject planet;

    /// <summary>
    /// Position / Rotation constraints
    /// </summary>
    public Transformation3D Constraints;

    /// <summary>
    /// Invert Positions / Rotations
    /// </summary>
    public Transformation3D InvertTransform;

    /// <summary>
    /// SetDefaultsTo lets you switch in one click between 3 different tracking modes â€“ hands, face and object tracking
    /// </summary>
    [BaseAction.ShowAtFirst]
    public Defaults SetDefaultsTo = Defaults.HandTracking;

    #endregion

    #region Private Fields

    [SerializeField]
    [HideInInspector]
    private Defaults _lastDefaults = Defaults.HandTracking;

    private bool _actionTriggered = false;

    private Vector3 rayHitCoordinates = new Vector3(0, 0, 0);
    private Vector3 localHit = new Vector3(0, 0, 0);
    private float lastVecZ = 0;
    private float lastVecY = 0;
    private float lastVecX = 0;

    #endregion

    #region Ctor

    public HandTrackingAction() : base()
    {
        Constraints = new Transformation3D();
        InvertTransform = new Transformation3D();
    }

    #endregion

    #region Public methods


    /// <summary>
    /// Sets the default trigger values (for the triggers set in SetDefaultTriggers() )
    /// </summary>
    /// <param name='index'>
    /// Index of the trigger.
    /// </param>
    /// <param name='trigger'>
    /// A pointer to the trigger for which you can set the default rules.
    /// </param>
    public override void SetDefaultTriggerValues(int index, Trigger trigger)
    {
            switch (index)
            {
                case 0:
                    trigger.FriendlyName = "Start Event";
                    ((EventTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<HandDetectedRule>() };
                    break;
                case 1:
                    ((TrackTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<HandTrackingRule>() };
                    break;
                case 2:
                    trigger.FriendlyName = "Stop Event";
                    ((EventTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<HandLostRule>() };
                    break;
            }
    }

    /// <summary>
    /// Updates the inspector.
    /// </summary>
    public override void UpdateInspector()
    {
        if (_lastDefaults != SetDefaultsTo)
        {
            CleanSupportedTriggers();
            SupportedTriggers = null;
            InitializeSupportedTriggers();
            _lastDefaults = SetDefaultsTo;
        }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Sets the default triggers for that action.
    /// </summary>
    override protected void SetDefaultTriggers()
    {
        SupportedTriggers = new Trigger[3]{
            AddHiddenComponent<EventTrigger>(),
            AddHiddenComponent<TrackTrigger>(),
            AddHiddenComponent<EventTrigger>()};
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        updateVirtualWorldBoxCenter();

        ProcessAllTriggers();

        //Start Event
        if (!_actionTriggered && SupportedTriggers[0].Success)
        {
            _actionTriggered = true;
        }

        //Stop Event
        if (_actionTriggered && SupportedTriggers[2].Success)
        {
            _actionTriggered = false;
        }

        if (!_actionTriggered)
        {
            return;
        }

        TrackTrigger trgr = (TrackTrigger)SupportedTriggers[1];

        if (trgr.Success)
        {

            // Translation:
            {
                Vector3 vec = trgr.Position;

                // Be sure we have valid values:
                if (VirtualWorldBoxDimensions.x <= 0)
                {
                    VirtualWorldBoxDimensions.x = 1;
                }

                if (VirtualWorldBoxDimensions.y <= 0)
                {
                    VirtualWorldBoxDimensions.y = 1;
                }

                if (VirtualWorldBoxDimensions.z <= 0)
                {
                    VirtualWorldBoxDimensions.z = 1;
                }

                // Get the relative position in the virtual box:
                float left = VirtualWorldBoxCenter.x - (VirtualWorldBoxDimensions.x) / 2;
                float top = VirtualWorldBoxCenter.y - (VirtualWorldBoxDimensions.y) / 2;
                float back = VirtualWorldBoxCenter.z - (VirtualWorldBoxDimensions.z) / 2;

                vec.x = (vec.x * VirtualWorldBoxDimensions.x);
                vec.y = (vec.y * VirtualWorldBoxDimensions.y);
                vec.z = (vec.z * VirtualWorldBoxDimensions.z);

                //invert
                if (InvertTransform.Position.X)
                {
                    vec.x = left + VirtualWorldBoxDimensions.x - vec.x;
                }
                else
                {
                    vec.x += left;
                }

                if (InvertTransform.Position.Y)
                {
                    vec.y = top + VirtualWorldBoxDimensions.y - vec.y;
                }
                else
                {
                    vec.y += top;
                }

                if (InvertTransform.Position.Z)
                {
                    vec.z = back + VirtualWorldBoxDimensions.z - vec.z;
                }
                else
                {
                    vec.z += back;
                }

                // Use the flags to indicate which axis are active
                if (Constraints.Position.X)
                {
                    vec.x = this.gameObject.transform.localPosition.x;
                }

                if (Constraints.Position.Y)
                {
                    vec.y = this.gameObject.transform.localPosition.y;
                }

                if (Constraints.Position.Z)
                {
                    vec.z = this.gameObject.transform.localPosition.z;
                }

                Vector3 handpos = this.gameObject.transform.position;
                Vector3 handpos_local = this.gameObject.transform.localPosition;

                Ray ray = new Ray(new Vector3(handpos.x, handpos.y, handpos.z-50), (planet.transform.position - handpos).normalized);
                RaycastHit hit;
                // Casts the ray and get the first game object hit
                Physics.Raycast(ray, out hit);
                // we hit the planet -> set target
                if (hit.transform && hit.transform.gameObject == planet)
                {
                    rayHitCoordinates = hit.point;
                    localHit = transform.InverseTransformPoint(hit.point);
                    //Debug.Log(localHit);
                }
               
                float planetradius = planet.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * planet.transform.localScale.x;
                float distance = Vector3.Distance(planet.transform.position, transform.position);
              
                //Debug.Log("distance: " + distance);
                //Debug.Log("planetradius: " + planetradius);
                //Debug.Log("ray hit:" + rayHitCoordinates);

                Vector3 currentVec = this.gameObject.transform.position;
               


                //Debug.Log("handpos_local:" + handpos_local);
                //Debug.Log("vec_z:" + vec.z);
                //if (rayHitCoordinates.z >= handpos.z || vec.z <= handpos_local.z)
                if (distance > planetradius+3 || vec.z <= handpos_local.z)
                {
                    this.gameObject.transform.localPosition = new Vector3(vec.x, vec.y, vec.z);
                    lastVecX = vec.x;
                    lastVecY = vec.y;
                    lastVecZ = vec.z;
                    //Debug.Log("vec: " + vec);
                    //Debug.Log("handpos_local: " + handpos_local);
                    //Debug.Log("ray hit:" + rayHitCoordinates);
                }
                else
                {
                    /*if (distance > planetradius)
                    {
                        this.gameObject.transform.localPosition = new Vector3(vec.x, vec.y, lastVecZ);
                        lastVecX = vec.x;
                        lastVecY = vec.y;
                    }
                    else*/
                        this.gameObject.transform.localPosition = new Vector3(lastVecX, lastVecY, lastVecZ);    
                }
            }
        }
    }


    #endregion

    #region Nested Types

    /// <summary>
    /// Default trackig modes that can be selected by SetDefaultsTo
    /// </summary>
    public enum Defaults
    {
        HandTracking
    }


    #endregion

}
