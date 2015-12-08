
using UnityEngine;
using RSUnityToolkit;

public class RotateCam : VirtualWorldBoxAction
{

    #region Public Fields

    public GameObject planet;
    public Camera cam;

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
    Vector3 camAngles_ini;

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

    void start()
    {
        camAngles_ini = cam.transform.eulerAngles;
    }

    void Update()
    {
        updateVirtualWorldBoxCenter();

        ProcessAllTriggers();

        TrackTrigger trgr = (TrackTrigger)SupportedTriggers[1];
        Vector3 handpos = trgr.Position;
 
        if (trgr.Success && handpos.x <= 0.4)
        {
            // Rotation
            {
                //current angles
                Quaternion angles = trgr.RotationQuaternion;
                Vector3 eulerAngles_hand = angles.eulerAngles;
                Vector3 camAngles = cam.transform.eulerAngles;

                if (_lastX == 0)
                    _lastX = eulerAngles_hand.x;

                if (_lastY == 0)
                    _lastY = eulerAngles_hand.y;

                //fixed hand position
                if (eulerAngles_hand.y < ThreshRightTurn)
                    //turn right
                    deltaY = -0.2f;
                else if (eulerAngles_hand.y > ThreshLeftTurn)
                    //turn left
                    deltaY = 0.2f;
                else
                    deltaY = 0;


                if (deltaY == 0 && (eulerAngles_hand.x > 310 && eulerAngles_hand.x < 340))
                {
                    //rotate to the player
                    if ((360 - (MaxPosX - camAngles_ini.x)+1 < (camAngles.x - 0.2f)) || camAngles.x <= camAngles_ini.x + MaxPosX)
                        deltaX = -0.2f;
                    else
                        deltaX = 0;
                }
                else if (deltaY == 0 && (eulerAngles_hand.x > 20) && (eulerAngles_hand.x < 310)){
                    //rotate away from the player
                    if ((360 - (MaxPosX - camAngles_ini.x) <= camAngles.x) || camAngles.x + 0.2f < camAngles_ini.x + MaxPosX-1)
                        deltaX = 0.2f;
                    else
                        deltaX = 0;
                }else
                    deltaX = 0;

                
                //Rotation and positioning of camera
                Quaternion cameraRotation = Quaternion.Euler(_lastX - deltaX, _lastY + deltaY, 0);
                cam.transform.rotation = cameraRotation;

                Vector3 cameraPosition = cameraRotation * new Vector3(0, 0, -185) + planet.transform.position;
                cam.transform.position = cameraPosition;

                _lastY = _lastY + deltaY;
                _lastX = _lastX + deltaX;
            }
        }
   }
    #endregion
}
#endregion