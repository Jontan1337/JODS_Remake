using Mirror;

public class WeaponStats : NetworkBehaviour
{
    public int damage;
    public float fireRate;
    [SyncVar] public int ammunition;
    [SyncVar] public int magazineCurrent;
	public int magazineMax;
	public float reloadTime;
	public float zoom;
	public float weaponRange;
	public float minAccuracy;
	public float recoil;
    [SyncVar] public bool isAutomatic;
	public float accuracy;
	public int multiShot;
	public float recoilAmountV;
	public float recoilAmountR;
	public bool isStarterWeapon;
}
