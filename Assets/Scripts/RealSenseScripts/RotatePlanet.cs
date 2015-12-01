
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RSUnityToolkit;

/// <summary>
/// Activate action: links the transformation of the associated Game Object to the real world tracked source
/// </summary>
public class RotatePlanet : VirtualWorldBoxAction
{

    #region Public Fields

    public float SmoothingFactor = 0;

    public SmoothingUtility.SmoothingTypes SmoothingType = SmoothingUtility.SmoothingTypes.Weighted;

    public Transformation3D InvertTransform;

    
    [SerializeField]
    private float RotationSpeed = 1;
    [SerializeField]
    private float MaxPosX = 30;
    [SerializeField]
    private float ThreshRightTurn = 20;
    [SerializeField]
    private float ThreshLeftTurn = 70;

    private float _lastX = 0f;
    private float _lastY = 0f;
    private float deltaY = 0;
    private float deltaX = 0;
    Quaternion angles;

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

    private SmoothingUtility _rotationSmoothingUtility = new SmoothingUtility();

    private bool initiate = true;

    Vector3 eulerAngles2;

    Vector3 objectAngles_ini;

    #endregion

    #region Ctor

    public RotatePlanet() : base()
    {
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
        if (SetDefaultsTo == Defaults.HandTracking)
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
        else if (SetDefaultsTo == Defaults.FaceTracking)
        {
            switch (index)
            {
                case 0:
                    trigger.FriendlyName = "Start Event";
                    ((EventTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<FaceDetectedRule>() };
                    break;
                case 1:
                    ((TrackTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<FaceTrackingRule>() };
                    break;
                case 2:
                    trigger.FriendlyName = "Stop Event";
                    ((EventTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<FaceLostRule>() };
                    break;
            }
        }
        else if (SetDefaultsTo == Defaults.ObjectTracking)
        {
            switch (index)
            {
                case 0:
                    trigger.FriendlyName = "Start Event";
                    ((EventTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<ObjectDetectedRule>() };
                    break;
                case 1:
                    ((TrackTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<ObjectTrackingRule>() };
                    break;
                case 2:
                    trigger.FriendlyName = "Stop Event";
                    ((EventTrigger)trigger).Rules = new BaseRule[1] { AddHiddenComponent<ObjectLostRule>() };
                    break;
            }
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

    void Update()
    {
        updateVirtualWorldBoxCenter();

        ProcessAllTriggers();

        TrackTrigger trgr = (TrackTrigger)SupportedTriggers[1];
        
        if (trgr.Success)
        {
            // Rotation
            {

                Quaternion angles = trgr.RotationQuaternion;

                if (initiate)
                {
                    //initial value
                    eulerAngles2 = angles.eulerAngles;
                    objectAngles_ini = this.gameObject.transform.eulerAngles;
                   
                    initiate = false;
                
                    //Debug.Log("initial x:" + eulerAngles2.x);
                    //Debug.Log("initial y:" + eulerAngles2.y);
                    //Debug.Log("initial object angle x: " + objectAngles_ini.x);
                }

                Vector3 eulerAngles3 = angles.eulerAngles;
                Vector3 objectAngles = this.gameObject.transform.eulerAngles;

                //turn in reference to the initial hand position
                /*if (deltaX == 0 && (eulerAngles3.y < (eulerAngles2.y - 10)))
                {
                    //turn left
                    deltaY = -RotationSpeed + 0.5f;

                }
                else if (deltaX == 0 && (eulerAngles3.y > (eulerAngles2.y + 10)))
                {
                    //turn right
                    deltaY = RotationSpeed - 0.5f;
                }
                else
                {
                    deltaY = 0;
                }*/


                //2nd case: fixed hand position
                if (eulerAngles3.y < ThreshRightTurn)
                {
                    //turn right
                    deltaY = RotationSpeed - 0.5f;
                }
                else if (eulerAngles3.y > ThreshLeftTurn)
                {
                    //turn left
                    deltaY = -RotationSpeed + 0.5f;
                }
                else
                {
                    deltaY = 0;
                }

                //Debug.Log("euler x:" + eulerAngles3.x);
                //Debug.Log("euler y:" + eulerAngles3.y);
               // Debug.Log("object x:" + objectAngles.x);
                //Debug.Log("delta y:" + deltaY);

                if (deltaY == 0 && (eulerAngles3.x > 310 && eulerAngles3.x < 340))
                {
                    //turn to the player
                    if ((360 - (MaxPosX - objectAngles_ini.x) < (objectAngles.x - RotationSpeed + 0.5f)) || objectAngles.x <= objectAngles_ini.x + MaxPosX)
                    {
                        Debug.Log("to the player");
                        deltaX = -RotationSpeed + 0.5f;
                    }
                    else
                        deltaX = 0;
                }
                else if (deltaY == 0 && (eulerAngles3.x > 20) && (eulerAngles3.x < 310))
                {
                    //away from the player
                    if ((360 - (MaxPosX - objectAngles_ini.x) <= objectAngles.x) || objectAngles.x + (RotationSpeed - 0.5f) <= objectAngles_ini.x + MaxPosX) { 
                    deltaX = RotationSpeed - 0.5f;
                        Debug.Log("away: " + deltaX);
                    }
                else
                    deltaX = 0;
                }
                else
                {
                    deltaX = 0;
                }

                //Smoothing
                if (SmoothingFactor > 0)
                {
                    angles = _rotationSmoothingUtility.ProcessSmoothing(SmoothingType, SmoothingFactor, angles);
                }

                Vector3 eulerAngles = angles.eulerAngles;

                if (_lastX == 0)
                {
                    _lastX = angles.eulerAngles.x;
                }
                if (_lastY == 0)
                {
                    _lastY = angles.eulerAngles.y;
                }


                if (!float.IsNaN(eulerAngles.x) && !float.IsNaN(eulerAngles.y) && !float.IsNaN(eulerAngles.z))
                {
                    
                    //invert
                    if (InvertTransform.Rotation.X)
                     {
                         eulerAngles.x = -eulerAngles.x;
                     }

                     if (InvertTransform.Rotation.Y)
                     {
                         eulerAngles.y = -eulerAngles.y;
                     }

                     if (InvertTransform.Rotation.Z)
                     {
                         eulerAngles.z = -eulerAngles.z;
                     }
                    

                        eulerAngles.z = this.gameObject.transform.localRotation.eulerAngles.z;
                        Vector3 vec = new Vector3(_lastX + deltaX, _lastY + deltaY, eulerAngles.z);
                    // this.gameObject.transform.localRotation = Quaternion.Euler(vec);
                    if(deltaY != 0)
                    {
                        Vector3 vec2 = new Vector3(0, 1, 0);
                        this.gameObject.transform.RotateAround(transform.position, vec2, _lastY - (_lastY + deltaY));
                    }
                    else if(deltaX != 0)
                    {
                        Vector3 vec2 = new Vector3(1, 0, 0);
                        this.gameObject.transform.RotateAround(transform.position, vec2, _lastX - (_lastX - deltaX));
                    }
                    _lastY = _lastY + deltaY;
                    _lastX = _lastX + deltaX;
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
        FaceTracking,
        HandTracking,
        ObjectTracking
    }



    #endregion

}
