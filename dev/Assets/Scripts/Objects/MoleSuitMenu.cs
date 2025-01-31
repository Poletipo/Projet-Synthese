﻿using UnityEngine;

public class MoleSuitMenu : MonoBehaviour {
    private Camera cam;


    private bool isSelected = false;
    private Vector3 suitRotation;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        suitRotation = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1")) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 10)) {
                if (hit.collider.gameObject == gameObject) {
                    isSelected = true;
                }
            }
        }
        else if (Input.GetButtonUp("Fire1")) {
            isSelected = false;
        }

        if (isSelected) {
            suitRotation += new Vector3(0, -Input.GetAxis("Mouse X"), 0) * 10;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(suitRotation);
    }

}
