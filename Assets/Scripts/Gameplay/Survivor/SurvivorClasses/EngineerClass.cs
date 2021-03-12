using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerClass : SurvivorClass
{
    PlaceItem place = new PlaceItem();
    Equipment equipment;

    private void Start()
    {
        place.look = GetComponent<LookController>();
    }

    public override void ActiveAbility()
    {
        if (true)
        {

        }
        place.Place(abilityObject, gameObject);

        //equipment.Svr_Equip(abilityObject, EquipmentType.Special);
        //abilityActivatedSuccesfully = true;
    }

}
