using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class UabFileUtil {
    public static bool IsFileExist(string path) {
        return File.Exists(path);
    }

    public static int GetFileSize(string path) {
        if (!IsFileExist(path)) {
            return 0;
        }
        FileInfo fi = new FileInfo(path);
        return (int)fi.Length;
    }

    private static string genMd5Str(byte[] bt) {
        StringBuilder sc = new StringBuilder();
        for (int i = 0; i < bt.Length; i++) {
            sc.Append(bt[i].ToString("x2"));
        }
        return sc.ToString();
    }

    public static string CalcDataMD5(byte[] data) {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] bt = md5.ComputeHash(data);
        return genMd5Str(bt);
    }

    public static string CalcFileMD5(string path) {
        if (!IsFileExist(path)) {
            return null;
        }
        FileInfo fi = new FileInfo(path);
        byte[] bt = null;
        using (FileStream fs = fi.OpenRead()) {
            MD5 md5 = new MD5CryptoServiceProvider();
            bt = md5.ComputeHash(fs);
        }
        return genMd5Str(bt);
    }
    public static string CalcFileSHA1(string path) {
        if (!IsFileExist(path)) {
            return null;
        }
        FileInfo fi = new FileInfo(path);
        byte[] bt = null;
        using (FileStream fs = fi.OpenRead()) {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            bt = sha1.ComputeHash(fs);
        }
        StringBuilder sc = new StringBuilder();
        for (int i = 0; i < bt.Length; i++) {
            sc.Append(bt[i].ToString("x2"));
        }
        return sc.ToString();
    }

    public static string PathCombine(params string[] args) {
        if (args.Length == 0) {
            return "";
        }
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < args.Length; i++) {
            if (string.IsNullOrEmpty(args[i])) {
                continue;
            }
            string s = args[i].Replace('\\', '/');
            sb.Append(s.EndsWith("/") ? s : s + "/");
        }

        string rtn = sb.ToString();
        return rtn.Substring(0, rtn.Length-1);
    }

    public static void CopyDirectory(string src, string dst, bool recreate = false, string ext = "*.*") {
        if (!Directory.Exists(src)) {
            return;
        }
        if (Directory.Exists(dst) && recreate) {
            Directory.Delete(dst, true);
        }
        if (!Directory.Exists(dst)) {
            Directory.CreateDirectory(dst);
        }

        string[] files = Directory.GetFiles(src, ext, SearchOption.AllDirectories);
        foreach (string file in files) {
            string filepath = file.Replace('\\', '/');
            string relativepath = filepath.Substring(src.Length + 1);
            string dstpath = UabFileUtil.PathCombine(dst, relativepath);
            string dstdir = Path.GetDirectoryName(dstpath);
            if (!Directory.Exists(dstdir)) {
                Directory.CreateDirectory(dstdir);
            }
            File.Copy(filepath, dstpath, true);
        }
    }

    public static void CopyFile(string src, string dst) {
        if (!File.Exists(src)) {
            return;
        }
        string dir = Path.GetDirectoryName(dst);
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        File.Copy(src, dst, true);
    }

    public static void EnsureDir(string dir, bool recreate = false) {
        if (Directory.Exists(dir) && recreate) {
            Directory.Delete(dir, true);
        }
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
    }

    public static void Move(string src, string dst) {
        if (File.Exists(dst)) {
            File.Delete(dst);
        }
        File.Move(src, dst);
    }
}
