using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEmployee : Bot
{
    public ShelfScript NeededShelf;
    public bool isCarrying = false;
    public int carryMax = 3;
    public int carryCount = 0;
    
    public bool employeeSelected = false;

    private void Start()
    {
        base.init();
    }

    protected override void Update()
    {
        base.Update();
        mouseClicked();
    }

    
    public virtual void mouseClicked()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null )
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    if(employeeSelected == false)
                    {
                        employeeSelected = true;
                        sprite.color = Color.green;
                    }

                }
                else
                {
                    employeeSelected = false;
                    sprite.color = Color.blue;
                }


            }
        }
    }
}
