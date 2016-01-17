using UnityEngine;
using RSUnityToolkit;


public class HandTrackingAction : VirtualWorldBoxAction
{

    #region Public Fields

    public GameObject planet;
    public Transformation3D Constraints;
    public Transformation3D InvertTransform;
    GrabController grab;
    #endregion

    #region Private Fields

    private bool _actionTriggered = false;
    private float lastVecZ = 0;
    private float lastVecY = 0;
    private float lastVecX = 0;

    //Smoothing parameters
    private SmoothingUtility _translationSmoothingUtility = new SmoothingUtility();
    private float SmoothingFactor = 20;
    private SmoothingUtility.SmoothingTypes SmoothingType = SmoothingUtility.SmoothingTypes.Weighted;

    #endregion

    #region Ctor

    public HandTrackingAction() : base()
    {
        Constraints = new Transformation3D();
        InvertTransform = new Transformation3D();    
    }

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

    #endregion

    #region Protected methods

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

                float planetradius = planet.GetComponent<MeshFilter>().mesh.bounds.size.x * 0.5f * planet.transform.localScale.x;
                float distance = Vector3.Distance(planet.transform.position, transform.position);        

                Vector3 currentVec = this.gameObject.transform.position;
                Vector3 handpos_local = this.gameObject.transform.localPosition;

                if (distance > planetradius+3)
                {
    
                    // smoothing:
                    if (SmoothingFactor > 0)
                    {
                        vec = _translationSmoothingUtility.ProcessSmoothing(SmoothingType, SmoothingFactor, vec);
                    }

					if(distance > (planetradius + 13)){
                    	this.gameObject.transform.localPosition = new Vector3(vec.x, vec.y, 130.0f);
					}else{
						this.gameObject.transform.localPosition = new Vector3(vec.x, vec.y, 130.0f + (distance - (planetradius + 13)));
					}
                    lastVecX = vec.x;
                    lastVecY = vec.y;
                    lastVecZ = vec.z;

                }
                else
                {
                    this.gameObject.transform.localPosition = new Vector3(lastVecX, lastVecY, lastVecZ);    
                }

                grab = GameObject.Find("HandOfGod").GetComponent<GrabController>();
                grab.moveVis(gameObject);
            }
        }
    }

    #endregion

}
