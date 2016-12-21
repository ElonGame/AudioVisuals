using System;
using GlmNet;

namespace Framework.OGL
{
    public static class GlmNetExtensions
    {
        public static mat4 PerformBillboard(this mat4 matrixToBillboard, float cameraX, float cameraY, float cameraZ, float objectX, float objectY, float objectZ)
        {
            mat4 billboardMatrix = mat4.identity();
            vec3 lookAt = new vec3();
            vec3 objToCamProj = new vec3();
            vec3 objToCam = new vec3();
            vec3 upAux = new vec3();
            float[] modelview = new float[16];
            float angleCosine;

            // objToCamProj is the vector in world coordinates from the 
            // local origin to the camera projected in the XZ plane
            objToCamProj[0] = cameraX - objectX;
            objToCamProj[1] = 0;
            objToCamProj[2] = cameraZ - objectZ;

            // This is the original lookAt vector for the object 
            // in world coordinates
            lookAt[0] = 0;
            lookAt[1] = 0;
            lookAt[2] = 1;

            // normalize both vectors to get the cosine directly afterwards
            objToCamProj = glm.normalize(objToCamProj);

            // easy fix to determine wether the angle is negative or positive
            // for positive angles upAux will be a vector pointing in the 
            // positive y direction, otherwise upAux will point downwards
            // effectively reversing the rotation.
            upAux = glm.cross(lookAt, objToCamProj);

            // compute the angle
            angleCosine = glm.dot(lookAt, objToCamProj);

            // perform the rotation. The if statement is used for stability reasons
            // if the lookAt and objToCamProj vectors are too close together then 
            // |angleCosine| could be bigger than 1 due to lack of precision
            if ((angleCosine < 0.99990) && (angleCosine > -0.9999))
            {
                billboardMatrix = glm.rotate(glm.acos(angleCosine) * glm.radians(180.0f / 3.14f), upAux);
            }

            // so far it is just like the cylindrical billboard. The code for the 
            // second rotation comes now
            // The second part tilts the object so that it faces the camera

            // objToCam is the vector in world coordinates from 
            // the local origin to the camera
            objToCam[0] = cameraX - objectX;
            objToCam[1] = cameraY - objectY;
            objToCam[2] = cameraZ - objectZ;

            // Normalize to get the cosine afterwards
            objToCam = glm.normalize(objToCam);

            // Compute the angle between objToCamProj and objToCam, 
            //i.e. compute the required angle for the lookup vector
            angleCosine = glm.dot(objToCamProj, objToCam);

            // Tilt the object. The test is done to prevent instability 
            // when objToCam and objToCamProj have a very small
            // angle between them
            if ((angleCosine < 0.9999) && (angleCosine > -0.9999))
            {
                if (objToCam[1] < 0)
                {
                    billboardMatrix = glm.rotate(glm.acos(angleCosine) * glm.radians(180.0f / 3.14f), new vec3(1, 0, 0));
                }
                else
                {
                    billboardMatrix = glm.rotate(glm.acos(angleCosine) * glm.radians(180.0f / 3.14f), new vec3(-1, 0, 0));
                }
            }

            return billboardMatrix;
        }
    }
}
