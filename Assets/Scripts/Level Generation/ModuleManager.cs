using System;
using System.Collections;
using System.Collections.Generic;
using LevelGeneration;
using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Holds module assets
    /// </summary>
    public class ModuleManager : MonoBehaviour
    {
        /// <summary>
        /// The module manager
        /// </summary>
        public static ModuleManager Instance { get; private set; }

        /// <summary>
        /// The modules
        /// </summary>
        public List<Module> modules;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else if (Instance != this)
                Destroy(gameObject);
        }
    }
}