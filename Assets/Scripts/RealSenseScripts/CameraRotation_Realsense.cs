using UnityEngine;
using RSUnityToolkit;

public class CameraRotation_Realsense : VirtualWorldBoxAction
{

    #region Public Fields

    public GameObject planet;
    public Camera cam;
    public float camSpeed = 10f;

    [SerializeField]
    private float MaxPosX = 30;
    [SerializeField]
    private float ThreshRightTurn = 22;
    [SerializeField]
    private float ThreshLeftTurn = 70;

    private float _lastX = 0f;
    private float _lastY = 0f;
    private float deltaY = 0;
    private float deltaX = 0;
    Quaternion angles;

    #endregion

    #region Public methods

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

                if (_lastX == 0)
                    _lastX = eulerAngles_hand.x;

                if (_lastY == 0)
                    _lastY = eulerAngles_hand.y;

                //fixed hand position
				if (deltaX == 0 && (eulerAngles_hand.y < ThreshRightTurn || eulerAngles_hand.y >= 280))
                {
                    //turn right
                    Vector3 verticalaxis = cam.transform.TransformDirection(Vector3.up);
                    cam.transform.RotateAround(planet.transform.position, verticalaxis, -camSpeed * Time.deltaTime);
                    deltaY = -0.2f;
                }
				else if (deltaX == 0 && (eulerAngles_hand.y > ThreshLeftTurn && eulerAngles_hand.y < 280))
                {
                    //turn left
                    Vector3 verticalaxis = cam.transform.TransformDirection(Vector3.down);
                    cam.transform.RotateAround(planet.transform.position, verticalaxis, -camSpeed * Time.deltaTime);
                    deltaY = 0.2f;
                }

                else
                    deltaY = 0;


                if (deltaY == 0 && (eulerAngles_hand.x > 310 && eulerAngles_hand.x < 340))
                {
                    //rotate to the player        
                        deltaX = -0.2f;
                        Vector3 horizontalaxis = cam.transform.TransformDirection(Vector3.right);
                        cam.transform.RotateAround(planet.transform.position, horizontalaxis, -camSpeed * Time.deltaTime);
                }
                else if (deltaY == 0 && (eulerAngles_hand.x > 20) && (eulerAngles_hand.x < 310)){
                    //rotate away from the player
                        deltaX = 0.2f;
                        Vector3 horizontalaxis = cam.transform.TransformDirection(Vector3.left);
                        cam.transform.RotateAround(planet.transform.position, horizontalaxis, -camSpeed * Time.deltaTime);
                }else
                    deltaX = 0;

            }
        }
   }
    #endregion
}
#endregion