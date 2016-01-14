using UnityEngine;
using System.Collections.Generic;
using Vuforia;

public class VirtualButtonEventHandler : MonoBehaviour,
                                         IVirtualButtonEventHandler
{
    public Material[] m_TeapotMaterials;

    private GameObject mTeapot;
    private List<Material> mActiveMaterials;

    private Game game;

    void Start()
    {
        // Register with the virtual buttons TrackableBehaviour
        VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vbs.Length; ++i)
        {
            vbs[i].RegisterEventHandler(this);
        }

        // Get handle to the teapot object
        mTeapot = transform.FindChild("teapot").gameObject;

        // The list of active materials
        mActiveMaterials = new List<Material>();

        game = GameObject.Find("_GameController").GetComponent<Game>();
    }

    public void OnButtonPressed(VirtualButtonAbstractBehaviour vb)
    {
        Debug.Log("OnButtonPressed::" + vb.VirtualButtonName);

        if (!IsValid())
        {
            return;
        }

        // Add the material corresponding to this virtual button
        // to the active material list:
        switch (vb.VirtualButtonName)
        {
            case "red":
                mActiveMaterials.Add(m_TeapotMaterials[0]);
                break;

            case "blue":
                mActiveMaterials.Add(m_TeapotMaterials[1]);
                break;

            case "yellow":
                mActiveMaterials.Add(m_TeapotMaterials[2]);
                break;

            case "green":
                mActiveMaterials.Add(m_TeapotMaterials[3]);
                break;
        }

        // Apply the new material:
        if (mActiveMaterials.Count > 0)
            mTeapot.GetComponent<Renderer>().material = mActiveMaterials[mActiveMaterials.Count - 1];
    }

    public void OnButtonReleased(VirtualButtonAbstractBehaviour vb)
    {
        if (!IsValid())
        {
            return;
        }

        // Remove the material corresponding to this virtual button
        // from the active material list:
        switch (vb.VirtualButtonName)
        {
            case "red":
                mActiveMaterials.Remove(m_TeapotMaterials[0]);
                break;

            case "blue":
                mActiveMaterials.Remove(m_TeapotMaterials[1]);
                break;

            case "yellow":
                mActiveMaterials.Remove(m_TeapotMaterials[2]);
                break;

            case "green":
                mActiveMaterials.Remove(m_TeapotMaterials[3]);
                break;
        }

        // Apply the next active material, or apply the default material:
        if (mActiveMaterials.Count > 0)
            mTeapot.GetComponent<Renderer>().material = mActiveMaterials[mActiveMaterials.Count - 1];
        else
            mTeapot.GetComponent<Renderer>().material = m_TeapotMaterials[4];
    }


    private bool IsValid()
    {
        // Check the materials and teapot have been set:
        return m_TeapotMaterials != null &&
                m_TeapotMaterials.Length == 5 &&
                mTeapot != null;
    }
}