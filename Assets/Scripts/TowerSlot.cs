using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    [SerializeField] private GameObject emptyVisual;

    private GameObject placedTower;
    private int purchasePrice;

    public bool Occupied => placedTower != null;
    public int Refund => purchasePrice;

    public bool Owns(Transform clickedObject)
    {
        return placedTower != null &&
               clickedObject.IsChildOf(placedTower.transform);
    }

    public void Build(GameObject prefab, Transform towerParent, int price)
    {
        if (Occupied) return;

        placedTower = Instantiate(
            prefab,
            transform.position + Vector3.up * 0.05f,
            Quaternion.identity,
            towerParent
        );

        purchasePrice = price;

        if (emptyVisual != null)
            emptyVisual.SetActive(false);
    }

    public int Sell()
    {
        if (!Occupied) return 0;

        int refund = purchasePrice;

        placedTower.SetActive(false);
        Destroy(placedTower);
        placedTower = null;
        purchasePrice = 0;

        if (emptyVisual != null)
            emptyVisual.SetActive(true);

        return refund;
    }
}