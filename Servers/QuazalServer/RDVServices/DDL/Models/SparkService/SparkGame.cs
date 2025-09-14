namespace QuazalServer.RDVServices.DDL.Models.SparkService
{
    public class SparkGame
    {
        public Gathering gathering { get; set; } // Quazal::_DDL_Gathering::Extract(a1, (unsigned int)a2);
        public bool unk { get; set; } // Quazal::ByteStream::operator>>(a1, (unsigned int)((_DWORD)a2 + 0x28));
        public uint param1 { get; set; } // Quazal::ByteStream::Extract(a1, (unsigned __int8 *)(unsigned int)((_DWORD)a2 + 0x2C), 4u, 1u);
        public uint param2 { get; set; } // Quazal::ByteStream::Extract(a1, (unsigned __int8 *)(unsigned int)((_DWORD)a2 + 0x30), 4u, 1u);
        public uint param3 { get; set; } // Quazal::ByteStream::Extract(a1, (unsigned __int8 *)(unsigned int)((_DWORD)a2 + 0x34), 4u, 1u);
        public uint param4 { get; set; } // Quazal::ByteStream::Extract(a1, (unsigned __int8 *)(unsigned int)((_DWORD)a2 + 0x38), 4u, 1u);
        public uint param5 { get; set; } // Quazal::ByteStream::Extract(a1, (unsigned __int8 *)(unsigned int)((_DWORD)a2 + 0x3C), 4u, 1u);
        public uint param6 { get; set; } // Quazal::ByteStream::Extract(a1, (unsigned __int8 *)(unsigned int)((_DWORD)a2 + 0x40), 4u, 1u);
        public uint param7 { get; set; } // Quazal::ByteStream::Extract(a1, (unsigned __int8 *)(unsigned int)((_DWORD)a2 + 0x44), 4u, 1u);
		
		/*v5 = a2[0x13];
		  if ( a2[0x14] != v5 )
			a2[0x14] = v5;
		  v24 = 0;
		  v6 = (Quazal::MemoryManager *)Quazal::ByteStream::Extract(a1, (unsigned __int8 *)&v24, 4u, 1u);
		  v7 = v24;
		  if ( v24 > 0x3FFFFFFF )
			std::vector<unsigned int,Quazal::MemAllocator<unsigned int>>::_Xlen((unsigned int)((_DWORD)a2 + 0x48));
		  v8 = a2[0x13];
		  v9 = 0;
		  if ( v8 )
			v9 = (a2[0x15] - v8) >> 2;
		  if ( v9 < v24 )
		  {
			v12 = 4 * v24;
			v13 = Quazal::MemoryManager::GetDefaultMemoryManager(v6);
			v15 = (Quazal::MemoryManager *)Quazal::MemoryManager::Allocate(
											 (Quazal::MemoryManager *)v13,
											 (Quazal::MemoryManager *)(v12 & 0xFFFFFFFC),
											 0x14B12D8u,
											 0LL,
											 7u,
											 v14);
			v16 = a2[0x13];
			v17 = a2[0x14];
			v18 = (int)v15;
			if ( v16 != v17 )
			{
			  v19 = (unsigned int)a2[0x13];
			  do
			  {
				v20 = (unsigned int)v19;
				v19 += 4LL;
				*(_DWORD *)v15 = *(_DWORD *)v20;
				v15 = (Quazal::MemoryManager *)((char *)v15 + 4);
			  }
			  while ( v17 != (_DWORD)v19 );
			}
			v21 = 0;
			if ( v16 )
			{
			  v22 = Quazal::MemoryManager::GetDefaultMemoryManager(v15);
			  Quazal::MemoryManager::Free((Quazal::MemoryManager *)v22, (Quazal::MemoryManager *)v16, (void *)7, v23);
			  v21 = 4 * ((int)(v17 - v16) >> 2);
			}
			v7 = v24;
			a2[0x13] = v18;
			a2[0x15] = v18 + v12;
			a2[0x14] = v21 + v18;
		  }
		  if ( v7 )
		  {
			v10 = 0;
			do
			{
			  ++v10;
			  Quazal::ByteStream::Extract((Quazal::ByteStream *)v4, v25, 4u, 1u);
			  std::vector<unsigned int,Quazal::MemAllocator<unsigned int>>::push_back((unsigned int)((_DWORD)a2 + 0x48), v25);
			}
			while ( v24 > v10 );
		  }*/
        public List<uint> extraParams { get; set; }
		
        public bool unk1 { get; set; } // Quazal::ByteStream::operator>>(v4, (unsigned int)((_DWORD)a2 + 0x58));
        public bool unk2 { get; set; } // Quazal::ByteStream::operator>>(v4, (unsigned int)((_DWORD)a2 + 0x59));
    }
}
