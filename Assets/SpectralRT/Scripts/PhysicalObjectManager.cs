using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SRT;

public class PhysicalObjectManager : MonoBehaviour {
    [System.NonSerialized]
    public Sphere[] spheres;

    [System.NonSerialized]
    public Disk[] disks;

    [System.NonSerialized]    
    public List<PhysicalMaterial> materialList;
    
    public PhysicalObjectManager() {
        materialList = new List<PhysicalMaterial>();
    }
    
    public void buildObjectList() {
        spheres = (Sphere[])GameObject.FindObjectsOfType(typeof(Sphere));
        disks = (Disk[])GameObject.FindObjectsOfType(typeof(Disk));
        
        for (int i=0; i < spheres.Length; i++) {
            PhysicalMaterial material = spheres[i].material;
            int index = NewMaterialIndex(material);
            if (index < 0) {
                materialList.Add(material);
                spheres[i].materialIndex = materialList.Count - 1;
            } else {
                spheres[i].materialIndex = index;
            }
        }
        
        for (int i=0; i < disks.Length; i++) {
            PhysicalMaterial material = disks[i].material;
            int index = NewMaterialIndex(material);
            if (index < 0) {
                materialList.Add(material);
                disks[i].materialIndex = materialList.Count - 1;
            } else {
                disks[i].materialIndex = index;
            }
        }
    }
    
    // return index of material in list or -1
    private int NewMaterialIndex(PhysicalMaterial newMaterial) {   
        for (int i=0; i < materialList.Count; i++) {
            if (newMaterial.Equals(materialList[i])) {
                return i;
            }
        }
        return -1;
    }
}
