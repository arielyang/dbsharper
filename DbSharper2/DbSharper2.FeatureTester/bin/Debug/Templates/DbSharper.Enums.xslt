﻿<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns:script="urn:scripts">
<xsl:import href="DbSharper.Scripts.xslt" />
<xsl:output omit-xml-declaration="yes" method="text" />
<xsl:param name="defaultNamespace" />
<xsl:template match="/">namespace <xsl:value-of select="$defaultNamespace" />.Enums
{<xsl:for-each select="/mapping/database/enumerations/enumeration">
	#region Enum <xsl:value-of select="@name" />

	/// &lt;summary&gt;
<xsl:value-of select="script:GetSummaryComment(@description, 1)" />
<xsl:if test="@description=''">	/// Summary of enum <xsl:value-of select="@name" />.</xsl:if>
	/// &lt;/summary&gt;<xsl:if test="@hasFlagsAttribute='true'">
	[global::System.Flags]</xsl:if>
	[global::System.Serializable]
	[global::System.Runtime.Serialization.DataContract]
	public enum <xsl:value-of select="@name" /> : <xsl:value-of select="script:CSharpAlias(@baseType)" />
	{<xsl:for-each select="member">
		/// &lt;summary&gt;
<xsl:value-of select="script:GetSummaryComment(@description, 2)" />
<xsl:if test="@description=''">		/// Summary of enum member <xsl:value-of select="@name" />.</xsl:if>
		/// &lt;/summary&gt;
		[global::System.ComponentModel.Description("<xsl:value-of select="@description" />")]
		[global::System.Runtime.Serialization.EnumMember]
		<xsl:value-of select="@name" /> = <xsl:value-of select="@value" /><xsl:if test="position()!=last()">,<xsl:text></xsl:text></xsl:if><xsl:text>
	</xsl:text>
	</xsl:for-each>}
	
	#endregion
</xsl:for-each>}
</xsl:template>
</xsl:stylesheet>