using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace VertexAnimationTools_30 {

    [System.Serializable]
    public struct MeshSequenceInfo   {

        public enum StateEnum {
            Empty_path,
            File_not_found,
            Ready
        }

        public enum SortModeEnum {
            ByDate,
            ByNumber
        }

        [System.Serializable]
        public struct ObjFileInfo {
            public string FileName;
            public int Hash;
            public int ParsedNumber;
            public FileInfo fi;
        }

        public StateEnum State;
        public ObjFileInfo[] infos;
        public string SequenceName;
        public string DirectoryPath;
        //public string Date;
        SortModeEnum SortMode ;

        public MeshSequenceInfo( string pathToAnyObj, SortModeEnum sortMode) {
            SortMode = sortMode;
            State = StateEnum.Empty_path;
            SequenceName = "";
            DirectoryPath = "";
            infos = new ObjFileInfo[0];
           // Date = System.DateTime.Now.ToString();

            if (string.IsNullOrEmpty(pathToAnyObj)) {
                return;
            }

            if (!File.Exists(pathToAnyObj)) {
                State = StateEnum.File_not_found;
                return;
            }

            DirectoryInfo di = (new FileInfo(pathToAnyObj)).Directory;
            FileInfo[] fis = di.GetFiles("*.obj");
            DirectoryPath = di.FullName;
            infos = new ObjFileInfo[fis.Length];
             
            for (int i = 0; i < infos.Length; i++) {
                infos[i] = new ObjFileInfo();
                infos[i].fi = fis[i];
                infos[i].FileName = Path.GetFileNameWithoutExtension(fis[i].FullName);
                infos[i].ParsedNumber = GetNumberFromName(infos[i].FileName, ref infos[i].Hash);
            }

            if (SortMode == SortModeEnum.ByDate) {
                System.Array.Sort(infos, InfosDateComparer);
            } else {
                System.Array.Sort(infos, InfosNumberComparer);
            }
            SequenceName = string.Format("[{0}-{1}]", infos[0].FileName, infos[infos.Length-1].FileName );
             
            State = StateEnum.Ready;
           
        }

        public string ShortInfo {
            get {
                if (State != StateEnum.Ready) {
                    return State.ToString().Replace('_',' ');
                } else {
                    return string.Format("{0} {1} frames", SequenceName, Count.ToString());
                }
            }
        }

        public int Count {
            get {
                if (infos == null) {
                    return 0;
                } else {
                    return infos.Length;
                }
            }
        }

        public ObjFileInfo this[int index] {
            get {
                return infos[index];
            }
        }

        int InfosNumberComparer(ObjFileInfo a, ObjFileInfo b) {
            if (a.ParsedNumber == b.ParsedNumber) {
                return a.Hash - b.Hash;
            } else {
                return   a.ParsedNumber - b.ParsedNumber;
            }
        }

        int InfosDateComparer(ObjFileInfo a, ObjFileInfo b) {
            int result;
            if (a.fi.LastWriteTime == b.fi.LastWriteTime) {
                result = 0;
            } else if (a.fi.LastWriteTime < b.fi.LastWriteTime) {
                result = -1;
            } else {
                result = 1;
            }

            return result;
        }

        int GetNumberFromName(string str, ref int prefixHash) {
            char[] nameChars = str.ToCharArray();
			int splitIdx = nameChars.Length;
            for (int i = nameChars.Length - 1; i >= 0; i--) {
				if (!char.IsDigit (nameChars [i])) {
					break;
				} else {
					splitIdx = i;
				}
            }

			string digitsPart = str.Substring( splitIdx );
			string prefixPart = str.Substring(0, splitIdx);
            prefixHash = prefixPart.GetHashCode();
            int index = 0;
            int.TryParse(digitsPart, out index);
            return index;
        }

 
    }
}
