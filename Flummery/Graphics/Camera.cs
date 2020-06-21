﻿using System;

using OpenTK;

namespace Flummery
{
    public enum ProjectionType
    {
        Orthographic,
        Perspective
    }

    public class Camera
    {
        public enum Direction
        {
            Forward,
            Backward,
            Left,
            Right,
            Up,
            Down
        }

        private Vector3 position;
        private Vector3 target = Vector3.Zero;

        private Matrix4 viewMatrix;

        private float yaw, pitch, roll;
        private float speed, rotationSpeed, zoomSpeed;
        private Matrix4 cameraRotation;

        ProjectionType projectionMode = ProjectionType.Perspective;

        public Vector3 Position => position;
        public Vector3 Up => cameraRotation.Up();
        public float Speed => speed;
        public float RotationSpeed => rotationSpeed;
        public float ZoomSpeed => zoomSpeed;

        public Matrix4 View => viewMatrix;
        public Matrix4 Rotation => cameraRotation;

        public ProjectionType ProjectionMode
        {
            get => projectionMode;
            set => projectionMode = value;
        }

        float zoom = 1.0f;

        public float Zoom
        {
            get => zoom;
            set
            {
                zoom = value;
                zoom = Math.Max(0.001f, zoom);
            }
        }

        public Camera() { ResetCamera(); }

        public void ResetCamera()
        {
            yaw = 0.0f;
            pitch = 0.0f;
            roll = 0.0f;

            speed = 10.0f;
            rotationSpeed = 0.005f;
            zoomSpeed = 0.001f;

            cameraRotation = Matrix4.Identity;

            position = new Vector3(0, 2.0f, 3.0f);
        }

        public void Update(float dt)
        {
            updateViewMatrix();
        }

        private void updateViewMatrix()
        {
            cameraRotation.NormaliseUp();
            cameraRotation.NormaliseRight();
            cameraRotation.NormaliseForward();

            cameraRotation *= Matrix4.CreateFromAxisAngle(cameraRotation.Up(), yaw);
            cameraRotation *= Matrix4.CreateFromAxisAngle(cameraRotation.Right(), pitch);
            cameraRotation *= Matrix4.CreateFromAxisAngle(cameraRotation.Forward(), roll);

            yaw = 0.0f;
            pitch = 0.0f;
            roll = 0.0f;

            position = target - (cameraRotation.Forward() * (projectionMode == ProjectionType.Perspective ? zoom : 25));

            viewMatrix = Matrix4.LookAt(position, target, cameraRotation.Up());
        }

        public void Frame(ModelMesh mesh)
        {
            target = Vector3.TransformVector(Vector3.TransformVector(mesh.BoundingBox.Centre, mesh.Parent.CombinedTransform), SceneManager.Current.Transform);
        }

        public void MoveCamera(Direction direction, float dt)
        {
            Vector3 v = Vector3.Zero;

            switch (direction)
            {
                case Direction.Forward:
                    v = cameraRotation.Forward() * dt;
                    break;

                case Direction.Backward:
                    v = -cameraRotation.Forward() * dt;
                    break;

                case Direction.Left:
                    v = -cameraRotation.Right() * dt;
                    break;

                case Direction.Right:
                    v = cameraRotation.Right() * dt;
                    break;

                case Direction.Up:
                    v = cameraRotation.Up() * dt;
                    break;

                case Direction.Down:
                    v = -cameraRotation.Up() * dt;
                    break;
            }

            position += speed * v;
        }

        public void SetPosition(float X = 0, float Y = 0, float Z = 0)
        {
            position.X = X;
            position.Y = Y;
            position.Z = Z;
        }

        public void SetPosition(Vector3 pos)
        {
            position = pos;
        }

        public void SetRotation(float yaw, float pitch, float roll)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
        }

        public void Rotate(float yaw = 0, float pitch = 0, float roll = 0, bool bApplySpeed = true)
        {
            this.yaw += yaw * (bApplySpeed ? rotationSpeed : 1);
            this.pitch += pitch * (bApplySpeed ? rotationSpeed : 1);
            this.roll += roll * (bApplySpeed ? rotationSpeed : 1);
        }

        public void Translate(float X = 0, float Y = 0, float Z = 0)
        {
            target += Vector3.TransformVector(new Vector3(X, Y, Z), cameraRotation);
        }

        public void SetActionScale(float speed)
        {
            this.speed = 1.25f * speed;
            rotationSpeed = 0.005f * speed;
            zoomSpeed = 0.001f * speed;
        }
    }
}