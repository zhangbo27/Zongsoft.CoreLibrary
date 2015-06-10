﻿/*
 * Authors:
 *   钟峰(Popeye Zhong) <zongsoft@gmail.com>
 *
 * Copyright (C) 2014-2015 Zongsoft Corporation <http://www.zongsoft.com>
 *
 * This file is part of Zongsoft.CoreLibrary.
 *
 * Zongsoft.CoreLibrary is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * Zongsoft.CoreLibrary is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with Zongsoft.CoreLibrary; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */

using System;
using System.IO;
using System.Collections.Generic;

namespace Zongsoft.IO
{
	public class LocalFileSystem : IFileSystem, Zongsoft.Services.IMatchable<string>
	{
		#region 单例字段
		public static readonly LocalFileSystem Instance = new LocalFileSystem();
		#endregion

		#region 构造函数
		private LocalFileSystem()
		{
		}
		#endregion

		#region 公共属性
		public string Schema
		{
			get
			{
				return FileSystem.Schema + ".local";
			}
		}

		public IFile File
		{
			get
			{
				return LocalFileService.Instance;
			}
		}

		public IDirectory Directory
		{
			get
			{
				return LocalDirectoryService.Instance;
			}
		}
		#endregion

		#region 匹配实现
		bool Services.IMatchable<string>.IsMatch(string parameter)
		{
			return string.Equals(this.Schema, parameter, StringComparison.OrdinalIgnoreCase);
		}

		bool Services.IMatchable.IsMatch(object parameter)
		{
			return string.Equals(this.Schema, (parameter as string), StringComparison.OrdinalIgnoreCase);
		}
		#endregion

		#region 嵌套子类
		private sealed class LocalDirectoryService : IDirectory
		{
			#region 单例字段
			public static readonly LocalDirectoryService Instance = new LocalDirectoryService();
			#endregion

			#region 私有构造
			private LocalDirectoryService()
			{
			}
			#endregion

			#region 公共方法
			public bool Create(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);

				if(System.IO.Directory.Exists(fullPath))
					return false;

				return System.IO.Directory.CreateDirectory(fullPath) != null;
			}

			public void Delete(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				System.IO.Directory.Delete(fullPath);
			}

			public void Delete(string path, bool recursive)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				System.IO.Directory.Delete(fullPath, recursive);
			}

			public void Move(string source, string destination)
			{
				var sourcePath = LocalPath.ToLocalPath(source);
				var destinationPath = LocalPath.ToLocalPath(destination);

				System.IO.Directory.Move(sourcePath, destinationPath);
			}

			public bool Exists(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.Exists(fullPath);
			}

			public DirectoryInfo GetInfo(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				var info = new System.IO.DirectoryInfo(fullPath);

				if(info == null || !info.Exists)
					return null;

				return new DirectoryInfo(info.FullName, name: info.Name, createdTime: info.CreationTime);
			}

			public IEnumerable<string> GetChildren(string path)
			{
				return this.GetChildren(path, null, false);
			}

			public IEnumerable<string> GetChildren(string path, string pattern, bool recursive = false)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.GetFileSystemEntries(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}

			public IEnumerable<string> GetDirectories(string path)
			{
				return this.GetDirectories(path, null, false);
			}

			public IEnumerable<string> GetDirectories(string path, string pattern, bool recursive = false)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.GetDirectories(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}

			public IEnumerable<string> GetFiles(string path)
			{
				return this.GetFiles(path, null, false);
			}

			public IEnumerable<string> GetFiles(string path, string pattern, bool recursive = false)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.Directory.GetFiles(fullPath, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
			}
			#endregion
		}

		private sealed class LocalFileService : IFile
		{
			#region 单例字段
			public static readonly LocalFileService Instance = new LocalFileService();
			#endregion

			#region 私有构造
			private LocalFileService()
			{
			}
			#endregion

			#region 公共方法
			public void Delete(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				System.IO.File.Delete(fullPath);
			}

			public bool Exists(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Exists(fullPath);
			}

			public void Copy(string source, string destination)
			{
				this.Copy(source, destination, true);
			}

			public void Copy(string source, string destination, bool overwrite)
			{
				var sourcePath = LocalPath.ToLocalPath(source);
				var destinationPath = LocalPath.ToLocalPath(destination);

				System.IO.File.Copy(sourcePath, destinationPath, overwrite);
			}

			public void Move(string source, string destination)
			{
				var sourcePath = LocalPath.ToLocalPath(source);
				var destinationPath = LocalPath.ToLocalPath(destination);

				System.IO.File.Move(sourcePath, destinationPath);
			}

			public FileInfo GetInfo(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				var info = new System.IO.FileInfo(fullPath);

				if(info == null || !info.Exists)
					return null;

				return new FileInfo(info.FullName, name: info.Name, size: info.Length, createdTime: info.CreationTime, modifiedTime: info.LastWriteTime);
			}

			public Stream Open(string path)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, FileMode.Open);
			}

			public Stream Open(string path, FileMode mode)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, mode);
			}

			public Stream Open(string path, FileMode mode, FileAccess access)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access);
			}

			public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
			{
				var fullPath = LocalPath.ToLocalPath(path);
				return System.IO.File.Open(fullPath, mode, access, share);
			}
			#endregion
		}
		#endregion
	}
}
