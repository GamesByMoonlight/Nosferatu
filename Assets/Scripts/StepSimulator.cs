using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;


public class StepSimulator : MonoBehaviour {

    
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.7f;
    [SerializeField] private float m_StepInterval = 5f;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
    [SerializeField] private AudioSource m_AudioSource;

    private float m_StepCycle = 0f;
    private float m_NextStep = 0f;
    private Camera m_Camera;
    private Vector3 m_OriginalCameraPosition;
    private FPSMouseLookController m_MovementController;

    // Use this for initialization
    private void Awake()
    {
        m_MovementController = GetComponent<FPSMouseLookController>();
    }

    void Start () {
        // Because this Start() may be called before FPSMouseLookController.  Keeping Camera.main repositioning in FPSMouseLookController.Start() - instead of Awake() - allows
        // us to Instantiate an Avatar over a network and deactive the FPS controller for other players before Start() is called, keeping the camera with the localPlayer only.
        StartCoroutine(WaitForCamera());
    }

    IEnumerator WaitForCamera()
    {
        m_Camera = GetComponentInChildren<Camera>();
        while(m_Camera == null)
        {
            yield return new WaitForSeconds(.1f);
            m_Camera = GetComponentInChildren<Camera>();
        }

        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_HeadBob.Setup(Camera.main, m_StepInterval);
    }

    // Update is called once per frame
    void Update () {
        if (!m_MovementController.PreviouslyGrounded && m_MovementController.Grounded)
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
        }
    }

    private void FixedUpdate()
    {
        ProgressStepCycle(m_MovementController.movementSettings.CurrentTargetSpeed, m_MovementController.CurrentInput);
        UpdateCameraPosition(m_MovementController.movementSettings.CurrentTargetSpeed);
    }

    private void ProgressStepCycle(float speed, Vector2 input)
    {
        if (m_MovementController.Velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
        {
            m_StepCycle += (m_MovementController.Velocity.magnitude + (speed * (m_MovementController.movementSettings.IsWalking ? 1f : m_RunstepLenghten))) *
                            Time.fixedDeltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!m_MovementController.Grounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip, m_AudioSource.volume);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    private void PlayLandingSound()
    {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }

    private void UpdateCameraPosition(float speed)
    {
        if (m_Camera == null)
            return;

        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_MovementController.Velocity.magnitude > 0 && m_MovementController.Grounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_MovementController.Velocity.magnitude +
                                    (speed * (m_MovementController.movementSettings.IsWalking ? 1f : m_RunstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }
}
