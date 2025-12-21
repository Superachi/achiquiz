using CardRoguelike.Exceptions;
using Godot;

namespace AchiQuiz
{
    public static class ResourceLoader
    {
        private static T TryLoadNewResource<T>(string path)
        {
            var loadedResource = GD.Load<Resource>(path);

            if (loadedResource == null)
            {
                throw new GameException($"Failed to load resource at path: '{path}'. Result was null (is the provided path correct?).");
            }

            if (loadedResource is not T typedResource)
            {
                throw new GameException($"Loaded resource at path '{path}' is not of type {typeof(T).Name}. Actual type: {loadedResource.GetType().Name}");
            }

            return typedResource;
        }

        public static T LoadResource<T>(string path)
        {
            return TryLoadNewResource<T>(path);
        }
    }
}