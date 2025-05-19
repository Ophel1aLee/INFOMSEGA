using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AnimalData", menuName = "AnimalData")]
public class AnimalData : ScriptableObject
{
    public int AnimalID;
    public string AnimalName;
    public string AnimalDescription;
    // Phylum: 门 or Class: 纲
    // most of the time (in Chordata脊索动物门, Arthropoda节肢动物门, Mollusca软体动物门) we use class like Mammalia哺乳纲, Amphibia两栖纲, Aves鸟纲, Insecta昆虫纲
    // otherwise we use phylum like Echinodermata棘皮动物门, Cnidaria刺胞动物门, Porifera多孔（海绵）动物门
    public string AnimalSpecies;
    public HabitatEnum AnimalHabitat;
    public DietEnum AnimalDiet;
    public string AnimalPicture;
}
