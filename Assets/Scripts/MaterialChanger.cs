using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MaterialChanger : MonoBehaviour
{

    public List<ConfigurablePart> configurableParts;

    public void ChangeConfiguration(int optionNumber)
    {
        foreach(ConfigurablePart cp in configurableParts)
        {
            cp.ChangeMaterial(optionNumber);
        }
    }

    [Serializable]
    public class ConfigurablePart
    {
        public MeshRenderer meshRenderer;
        public List<Material> options;

        public void ChangeMaterial(int index)
        {
            if (index >= options.Count)
                return;

            Material[] tmpMats = meshRenderer.materials;
            tmpMats[0] = options[index];
            meshRenderer.materials = tmpMats;
        }
    }
}
