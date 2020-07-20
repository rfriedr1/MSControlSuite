<?xml version='1.0' encoding='UTF-8'?>
<Project Type="Project" LVVersion="18008000">
	<Item Name="My Computer" Type="My Computer">
		<Property Name="NI.SortType" Type="Int">3</Property>
		<Property Name="server.app.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.control.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="server.tcp.enabled" Type="Bool">false</Property>
		<Property Name="server.tcp.port" Type="Int">0</Property>
		<Property Name="server.tcp.serviceName" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.tcp.serviceName.default" Type="Str">My Computer/VI Server</Property>
		<Property Name="server.vi.callsEnabled" Type="Bool">true</Property>
		<Property Name="server.vi.propertiesEnabled" Type="Bool">true</Property>
		<Property Name="specify.custom.address" Type="Bool">false</Property>
		<Item Name="ConradRelay8" Type="Folder">
			<Item Name="calc_CRC.vi" Type="VI" URL="../calc_CRC.vi"/>
			<Item Name="create_data_to_send.vi" Type="VI" URL="../create_data_to_send.vi"/>
			<Item Name="init_cards.vi" Type="VI" URL="../init_cards.vi"/>
			<Item Name="init_com.vi" Type="VI" URL="../init_com.vi"/>
			<Item Name="switches_to_number.vi" Type="VI" URL="../switches_to_number.vi"/>
			<Item Name="send_command.vi" Type="VI" URL="../send_command.vi"/>
			<Item Name="set_relays.vi" Type="VI" URL="../set_relays.vi"/>
			<Item Name="activate_single_relays.vi" Type="VI" URL="../activate_single_relays.vi"/>
			<Item Name="deactivate_single_relays.vi" Type="VI" URL="../deactivate_single_relays.vi"/>
			<Item Name="format_answer.vi" Type="VI" URL="../format_answer.vi"/>
			<Item Name="number_to_switches.vi" Type="VI" URL="../number_to_switches.vi"/>
			<Item Name="get_relays.vi" Type="VI" URL="../get_relays.vi"/>
		</Item>
		<Item Name="helpers" Type="Folder"/>
		<Item Name="main.vi" Type="VI" URL="../main.vi"/>
		<Item Name="testing.vi" Type="VI" URL="../testing.vi"/>
		<Item Name="Dependencies" Type="Dependencies">
			<Item Name="vi.lib" Type="Folder">
				<Item Name="Stall Data Flow.vim" Type="VI" URL="/&lt;vilib&gt;/Utility/Stall Data Flow.vim"/>
				<Item Name="VISA Configure Serial Port" Type="VI" URL="/&lt;vilib&gt;/Instr/_visa.llb/VISA Configure Serial Port"/>
				<Item Name="VISA Configure Serial Port (Instr).vi" Type="VI" URL="/&lt;vilib&gt;/Instr/_visa.llb/VISA Configure Serial Port (Instr).vi"/>
				<Item Name="VISA Configure Serial Port (Serial Instr).vi" Type="VI" URL="/&lt;vilib&gt;/Instr/_visa.llb/VISA Configure Serial Port (Serial Instr).vi"/>
				<Item Name="whitespace.ctl" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/whitespace.ctl"/>
				<Item Name="Trim Whitespace.vi" Type="VI" URL="/&lt;vilib&gt;/Utility/error.llb/Trim Whitespace.vi"/>
			</Item>
		</Item>
		<Item Name="Build Specifications" Type="Build">
			<Item Name="SwitchBoxConrad exe" Type="EXE">
				<Property Name="App_copyErrors" Type="Bool">true</Property>
				<Property Name="App_INI_aliasGUID" Type="Str">{9231806C-E54C-491B-8371-512130CB80FC}</Property>
				<Property Name="App_INI_GUID" Type="Str">{15925B9B-C3D8-426D-9D54-F344611E45A4}</Property>
				<Property Name="App_serverConfig.httpPort" Type="Int">8002</Property>
				<Property Name="Bld_autoIncrement" Type="Bool">true</Property>
				<Property Name="Bld_buildCacheID" Type="Str">{47EFD0BC-1E7E-4617-8CB4-0223A1136D4E}</Property>
				<Property Name="Bld_buildSpecName" Type="Str">SwitchBoxConrad exe</Property>
				<Property Name="Bld_excludeInlineSubVIs" Type="Bool">true</Property>
				<Property Name="Bld_excludeLibraryItems" Type="Bool">true</Property>
				<Property Name="Bld_excludePolymorphicVIs" Type="Bool">true</Property>
				<Property Name="Bld_localDestDir" Type="Path">../build</Property>
				<Property Name="Bld_localDestDirType" Type="Str">relativeToProject</Property>
				<Property Name="Bld_modifyLibraryFile" Type="Bool">true</Property>
				<Property Name="Bld_previewCacheID" Type="Str">{1185EB24-4631-4122-A6CE-BBE3C19F071F}</Property>
				<Property Name="Bld_version.build" Type="Int">9</Property>
				<Property Name="Bld_version.major" Type="Int">1</Property>
				<Property Name="Destination[0].destName" Type="Str">SwitchBoxConrad.exe</Property>
				<Property Name="Destination[0].path" Type="Path">../build/NI_AB_PROJECTNAME.exe</Property>
				<Property Name="Destination[0].path.type" Type="Str">relativeToProject</Property>
				<Property Name="Destination[0].preserveHierarchy" Type="Bool">true</Property>
				<Property Name="Destination[0].type" Type="Str">App</Property>
				<Property Name="Destination[1].destName" Type="Str">Support Directory</Property>
				<Property Name="Destination[1].path" Type="Path">../build/data</Property>
				<Property Name="Destination[1].path.type" Type="Str">relativeToProject</Property>
				<Property Name="DestinationCount" Type="Int">2</Property>
				<Property Name="Source[0].itemID" Type="Str">{DBDC54E7-6EEA-4DEF-AA9F-47D1C5E8D32A}</Property>
				<Property Name="Source[0].type" Type="Str">Container</Property>
				<Property Name="Source[1].destinationIndex" Type="Int">0</Property>
				<Property Name="Source[1].itemID" Type="Ref">/My Computer/main.vi</Property>
				<Property Name="Source[1].properties[0].type" Type="Str">Show menu bar</Property>
				<Property Name="Source[1].properties[0].value" Type="Bool">false</Property>
				<Property Name="Source[1].properties[1].type" Type="Str">Show vertical scroll bar</Property>
				<Property Name="Source[1].properties[1].value" Type="Bool">false</Property>
				<Property Name="Source[1].properties[2].type" Type="Str">Show horizontal scroll bar</Property>
				<Property Name="Source[1].properties[2].value" Type="Bool">false</Property>
				<Property Name="Source[1].properties[3].type" Type="Str">Show toolbar</Property>
				<Property Name="Source[1].properties[3].value" Type="Bool">false</Property>
				<Property Name="Source[1].properties[4].type" Type="Str">Show Abort button</Property>
				<Property Name="Source[1].properties[4].value" Type="Bool">false</Property>
				<Property Name="Source[1].propertiesCount" Type="Int">5</Property>
				<Property Name="Source[1].sourceInclusion" Type="Str">TopLevel</Property>
				<Property Name="Source[1].type" Type="Str">VI</Property>
				<Property Name="SourceCount" Type="Int">2</Property>
				<Property Name="TgtF_companyName" Type="Str">Ronny Friedrich</Property>
				<Property Name="TgtF_fileDescription" Type="Str">SwitchBoxConrad exe</Property>
				<Property Name="TgtF_internalName" Type="Str">SwitchBoxConrad exe</Property>
				<Property Name="TgtF_legalCopyright" Type="Str">Copyright © 2018 Ronny Friedrich</Property>
				<Property Name="TgtF_productName" Type="Str">SwitchBoxConrad exe</Property>
				<Property Name="TgtF_targetfileGUID" Type="Str">{9039104E-8AE8-4709-9307-C9B8ED128363}</Property>
				<Property Name="TgtF_targetfileName" Type="Str">SwitchBoxConrad.exe</Property>
				<Property Name="TgtF_versionIndependent" Type="Bool">true</Property>
			</Item>
		</Item>
	</Item>
</Project>
