<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:cac="urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2" xmlns:cbc="urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"
xmlns:ccts="urn:un:unece:uncefact:documentation:2" 
xmlns:clm54217="urn:un:unece:uncefact:codelist:specification:54217:2001" 
xmlns:clm5639="urn:un:unece:uncefact:codelist:specification:5639:1988"
xmlns:clm66411="urn:un:unece:uncefact:codelist:specification:66411:2001" 
xmlns:clmIANAMIMEMediaType="urn:un:unece:uncefact:codelist:specification:IANAMIMEMediaType:2003" xmlns:fn="http://www.w3.org/2005/xpath-functions"
xmlns:link="http://www.xbrl.org/2003/linkbase" 
xmlns:n1="urn:oasis:names:specification:ubl:schema:xsd:Invoice-2" 
xmlns:qdt="urn:oasis:names:specification:ubl:schema:xsd:QualifiedDatatypes-2"
xmlns:udt="urn:un:unece:uncefact:data:specification:UnqualifiedDataTypesSchemaModule:2" xmlns:xbrldi="http://xbrl.org/2006/xbrldi" xmlns:xbrli="http://www.xbrl.org/2003/instance" xmlns:xdt="http://www.w3.org/2005/xpath-datatypes"
xmlns:xlink="http://www.w3.org/1999/xlink" 
xmlns:xs="http://www.w3.org/2001/XMLSchema" 
xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
exclude-result-prefixes="cac cbc ccts clm54217 clm5639 clm66411 clmIANAMIMEMediaType fn link n1 qdt udt xbrldi xbrli xdt xlink xs xsd xsi">
	<xsl:decimal-format name="european" decimal-separator="," grouping-separator="." NaN=""/>
	<xsl:output version="4.0" method="html" indent="no" encoding="UTF-8" doctype-public="-//W3C//DTD HTML 4.01 Transitional//EN" doctype-system="http://www.w3.org/TR/html4/loose.dtd"/>
	<xsl:param name="SV_OutputFormat" select="'HTML'"/>
	<xsl:variable name="XML" select="/"/>
	<xsl:template match="/">
		<html>
			<head>
				<title/>
				<style type="text/css">body {
                                                                                  background-color: #FFFFFF;
                                                                                  font-family: 'Tahoma', "Times New Roman", Times, serif;
                                                                                  font-size: 11px;
                                                                                  color: #666666;
                                                                              }
                                                                              h1, h2 {
                                                                                  padding-bottom: 3px;
                                                                                  padding-top: 3px;
                                                                                  margin-bottom: 5px;
                                                                                  text-transform: uppercase;
                                                                                  font-family: Arial, Helvetica, sans-serif;
                                                                              }
                                                                              h1 {
                                                                                  font-size: 1.4em;
                                                                                  text-transform:none;
                                                                              }
                                                                              h2 {
                                                                                  font-size: 1em;
                                                                                  color: brown;
                                                                              }
                                                                              h3 {
                                                                                  font-size: 1em;
                                                                                  color: #333333;
                                                                                  text-align: justify;
                                                                                  margin: 0;
                                                                                  padding: 0;
                                                                              }
                                                                              h4 {
                                                                                  font-size: 1.1em;
                                                                                  font-style: bold;
                                                                                  font-family: Arial, Helvetica, sans-serif;
                                                                                  color: #000000;
                                                                                  margin: 0;
                                                                                  padding: 0;
                                                                              }
                                                                              hr {
                                                                                  height:2px;
                                                                                  color: #000000;
                                                                                  background-color: #000000;
                                                                                  border-bottom: 1px solid #000000;
                                                                              }
                                                                              p, ul, ol {
                                                                                  margin-top: 1.5em;
                                                                              }
                                                                              ul, ol {
                                                                                  margin-left: 3em;
                                                                              }
                                                                              blockquote {
                                                                                  margin-left: 3em;
                                                                                  margin-right: 3em;
                                                                                  font-style: italic;
                                                                               }
                                                                              a {
                                                                                  text-decoration: none;
                                                                                  color: #70A300;
                                                                              }
                                                                              a:hover {
                                                                                  border: none;
                                                                                  color: #70A300;
                                                                              }
                                                                              #despatchTable {
                                                                                  border-collapse:collapse;
                                                                                  font-size:11px;
                                                                                  float:right;
                                                                                  border-color:gray;
                                                                              }
                                                                              #ettnTable {
                                                                                  border-collapse:collapse;
                                                                                  font-size:11px;
                                                                                  border-color:gray;
                                                                              }
                                                                              #customerPartyTable {
                                                                                  border-width: 0px;
                                                                                  border-spacing:;
                                                                                  border-style: inset;
                                                                                  border-color: gray;
                                                                                  border-collapse: collapse;
                                                                                  background-color:
                                                                              }
                                                                              #customerIDTable {
                                                                                  border-width: 2px;
                                                                                  border-spacing:;
                                                                                  border-style: inset;
                                                                                  border-color: gray;
                                                                                  border-collapse: collapse;
                                                                                  background-color:
                                                                              }
                                                                              #customerIDTableTd {
                                                                                  border-width: 2px;
                                                                                  border-spacing:;
                                                                                  border-style: inset;
                                                                                  border-color: gray;
                                                                                  border-collapse: collapse;
                                                                                  background-color:
                                                                              }
                                                                              #lineTable {
                                                                                  border-width:2px;
                                                                                  border-spacing:;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  border-collapse: collapse;
                                                                                  background-color:;
                                                                              }
                                                                              #lineTableTd {
                                                                                  border-width: 1px;
                                                                                  padding: 1px;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  background-color: white;
                                                                              }
                                                                              #lineTableTr {
                                                                                  border-width: 1px;
                                                                                  padding: 0px;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  background-color: white;
                                                                                  -moz-border-radius:;
                                                                              }
                                                                              #lineTableDummyTd {
                                                                                  border-width: 1px;
                                                                                  border-color:white;
                                                                                  padding: 1px;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  background-color: white;
                                                                              }
                                                                              #lineTableBudgetTd {
                                                                                  border-width: 2px;
                                                                                  border-spacing:0px;
                                                                                  padding: 1px;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  background-color: white;
                                                                                  -moz-border-radius:;
                                                                              }
                                                                              #notesTable {
                                                                                  border-width: 2px;
                                                                                  border-spacing:;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  border-collapse: collapse;
                                                                                  background-color:
                                                                              }
                                                                              #notesTableTd {
                                                                                  border-width: 0px;
                                                                                  border-spacing:;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  border-collapse: collapse;
                                                                                  background-color:
                                                                              }
                                                                              table {
                                                                                  border-spacing:0px;
                                                                              }
                                                                              #budgetContainerTable {
                                                                                  border-width: 0px;
                                                                                  border-spacing: 0px;
                                                                                  border-style: inset;
                                                                                  border-color: black;
                                                                                  border-collapse: collapse;
                                                                                  background-color:;
                                                                              }
                                                                              td {
                                                                                  border-color:gray;
                                                                              }</style>
				<title>e-Fatura</title>
			</head>
			<body style="margin-left=0.6in; margin-right=0.6in; margin-top=0.79in; margin-bottom=0.79in">
				<xsl:for-each select="$XML">
					<table style="border-color:blue; " border="0" cellspacing="0px" width="800" cellpadding="0px">
						<tbody>
							<tr valign="top">
								<td width="40%">
									<br/>
									<table align="center" border="0" width="100%">
										<tbody>
											<hr/>
											<tr align="left">
												<xsl:for-each select="n1:Invoice">
													<xsl:for-each select="cac:AccountingSupplierParty">
														<xsl:for-each select="cac:Party">
															<td align="left">
																<xsl:if test="cac:PartyName">
																	<xsl:value-of select="cac:PartyName/cbc:Name"/>
																	<br/>
																</xsl:if>
																<xsl:for-each select="cac:Person">
																	<xsl:for-each select="cbc:Title">
																		<xsl:apply-templates/>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																	<xsl:for-each select="cbc:FirstName">
																		<xsl:apply-templates/>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																	<xsl:for-each select="cbc:MiddleName">
																		<xsl:apply-templates/>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																	<xsl:for-each select="cbc:FamilyName">
																		<xsl:apply-templates/>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																	<xsl:for-each select="cbc:NameSuffix">
																		<xsl:apply-templates/>
																	</xsl:for-each>
																</xsl:for-each>
															</td>
														</xsl:for-each>
													</xsl:for-each>
												</xsl:for-each>
											</tr>
											<tr align="left">
												<xsl:for-each select="n1:Invoice">
													<xsl:for-each select="cac:AccountingSupplierParty">
														<xsl:for-each select="cac:Party">
															<td align="left">
																<xsl:for-each select="cac:PostalAddress">
																	<xsl:for-each select="cbc:StreetName">
																		<xsl:apply-templates/>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																	<xsl:for-each select="cbc:BuildingName">
																		<xsl:apply-templates/>
																	</xsl:for-each>
																	<xsl:if test="cbc:BuildingNumber">
																		<span>
																			<xsl:text> No:</xsl:text>
																		</span>
																		<xsl:for-each select="cbc:BuildingNumber">
																			<xsl:apply-templates/>
																		</xsl:for-each>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:if>
																	<br/>
																	<xsl:for-each select="cbc:PostalZone">
																		<xsl:apply-templates/>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																	<xsl:for-each select="cbc:CitySubdivisionName">
																		<xsl:apply-templates/>
																	</xsl:for-each>
																	<span>
																		<xsl:text>/ </xsl:text>
																	</span>
																	<xsl:for-each select="cbc:CityName">
																		<xsl:apply-templates/>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																</xsl:for-each>
															</td>
														</xsl:for-each>
													</xsl:for-each>
												</xsl:for-each>
											</tr>
											<xsl:if test="//n1:Invoice/cac:AccountingSupplierParty/cac:Party/cac:Contact/cbc:Telephone or //n1:Invoice/cac:AccountingSupplierParty/cac:Party/cac:Contact/cbc:Telefax">
												<tr align="left">
													<xsl:for-each select="n1:Invoice">
														<xsl:for-each select="cac:AccountingSupplierParty">
															<xsl:for-each select="cac:Party">
																<td align="left">
																	<xsl:for-each select="cac:Contact">
																		<xsl:if test="cbc:Telephone">
																			<span>
																				<xsl:text>Tel: </xsl:text>
																			</span>
																			<xsl:for-each select="cbc:Telephone">
																				<xsl:apply-templates/>
																			</xsl:for-each>
																		</xsl:if>
																		<xsl:if test="cbc:Telefax">
																			<span>
																				<xsl:text> Fax: </xsl:text>
																			</span>
																			<xsl:for-each select="cbc:Telefax">
																				<xsl:apply-templates/>
																			</xsl:for-each>
																		</xsl:if>
																		<span>
																			<xsl:text>&#xA0;</xsl:text>
																		</span>
																	</xsl:for-each>
																</td>
															</xsl:for-each>
														</xsl:for-each>
													</xsl:for-each>
												</tr>
											</xsl:if>
											<xsl:for-each select="//n1:Invoice/cac:AccountingSupplierParty/cac:Party/cbc:WebsiteURI">
												<tr align="left">
													<td>
														<xsl:text>Web Sitesi: </xsl:text>
														<xsl:value-of select="."/>
													</td>
												</tr>
											</xsl:for-each>
											<xsl:for-each select="//n1:Invoice/cac:AccountingSupplierParty/cac:Party/cac:Contact/cbc:ElectronicMail">
												<tr align="left">
													<td>
														<xsl:text>E-Posta: </xsl:text>
														<xsl:value-of select="."/>
													</td>
												</tr>
											</xsl:for-each>
											<tr align="left">
												<xsl:for-each select="n1:Invoice">
													<xsl:for-each select="cac:AccountingSupplierParty">
														<xsl:for-each select="cac:Party">
															<td align="left">
																<span>
																	<xsl:text>Vergi Dairesi: </xsl:text>
																</span>
																<xsl:for-each select="cac:PartyTaxScheme">
																	<xsl:for-each select="cac:TaxScheme">
																		<xsl:for-each select="cbc:Name">
																			<xsl:apply-templates/>
																		</xsl:for-each>
																	</xsl:for-each>
																	<span>
																		<xsl:text>&#xA0; </xsl:text>
																	</span>
																</xsl:for-each>
															</td>
														</xsl:for-each>
													</xsl:for-each>
												</xsl:for-each>
											</tr>
											<xsl:for-each select="//n1:Invoice/cac:AccountingSupplierParty/cac:Party/cac:PartyIdentification">
												<tr align="left">
													<td>
														<xsl:value-of select="cbc:ID/@schemeID"/>
														<xsl:text>: </xsl:text>
														<xsl:value-of select="cbc:ID"/>
													</td>
												</tr>
											</xsl:for-each>
										</tbody>
									</table>
									<hr/>
								</td>
								<td width="20%" align="center" valign="middle">
									<br/>
									<br/>
									<img style="width:91px;" align="middle" alt="E-Fatura Logo"
									     src="data:image/jpeg;base64,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"/>

									<h1 align="center">
										<span style="font-weight:bold; ">
											<xsl:text>e-ARSIVFATURA</xsl:text>
										</span>
									</h1>
									<img style='max-width:200px;' alt='İmza' src='data:image/png;base64,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'/>
								</td>
								<td width="40%"><img style='max-width:300px;' align='right' alt='' src='data:image/png;base64,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'/></td>
							</tr>
							<tr style="height:118px; " valign="top">
								<td width="40%" align="right" valign="bottom">
									<table id="customerPartyTable" align="left" border="0" height="50%">
										<tbody>
											<tr style="height:71px; ">
												<td>
													<hr/>
													<table align="center" border="0">
														<tbody>
															<tr>
																<xsl:for-each select="n1:Invoice">
																	<xsl:for-each select="cac:AccountingCustomerParty">
																		<xsl:for-each select="cac:Party">
																			<td style="width:469px; " align="left">
																				<span style="font-weight:bold; ">
																					<xsl:text>SAYIN</xsl:text>
																				</span>
																			</td>
																		</xsl:for-each>
																	</xsl:for-each>
																</xsl:for-each>
															</tr>
															<tr>
																<xsl:for-each select="n1:Invoice">
																	<xsl:for-each select="cac:AccountingCustomerParty">
																		<xsl:for-each select="cac:Party">
																			<td style="width:469px; " align="left">
																				<xsl:if test="cac:PartyName">
																					<xsl:value-of select="cac:PartyName/cbc:Name"/>
																					<br/>
																				</xsl:if>
																				<xsl:for-each select="cac:Person">
																					<xsl:for-each select="cbc:Title">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:FirstName">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:MiddleName">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0; </xsl:text>
																						</span>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:FamilyName">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:NameSuffix">
																						<xsl:apply-templates/>
																					</xsl:for-each>
																				</xsl:for-each>
																			</td>
																		</xsl:for-each>
																	</xsl:for-each>
																</xsl:for-each>
															</tr>
															<tr>
																<xsl:for-each select="n1:Invoice">
																	<xsl:for-each select="cac:AccountingCustomerParty">
																		<xsl:for-each select="cac:Party">
																			<td style="width:469px; " align="left">
																				<xsl:for-each select="cac:PostalAddress">
																					<xsl:for-each select="cbc:StreetName">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:BuildingName">
																						<xsl:apply-templates/>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:BuildingNumber">
																						<span>
																							<xsl:text> No:</xsl:text>
																						</span>
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																					<br/>
																					<xsl:for-each select="cbc:Room">
																						<span>
																							<xsl:text>Kapı No:</xsl:text>
																						</span>
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																					<br/>
																					<xsl:for-each select="cbc:PostalZone">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:CitySubdivisionName">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>/ </xsl:text>
																						</span>
																					</xsl:for-each>
																					<xsl:for-each select="cbc:CityName">
																						<xsl:apply-templates/>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</xsl:for-each>
																				</xsl:for-each>
																			</td>
																		</xsl:for-each>
																	</xsl:for-each>
																</xsl:for-each>
															</tr>
															<xsl:for-each select="//n1:Invoice/cac:AccountingCustomerParty/cac:Party/cbc:WebsiteURI">
																<tr align="left">
																	<td>
																		<xsl:text>Web Sitesi: </xsl:text>
																		<xsl:value-of select="."/>
																	</td>
																</tr>
															</xsl:for-each>
															<xsl:for-each select="//n1:Invoice/cac:AccountingCustomerParty/cac:Party/cac:Contact/cbc:ElectronicMail">
																<tr align="left">
																	<td>
																		<xsl:text>E-Posta: </xsl:text>
																		<xsl:value-of select="."/>
																	</td>
																</tr>
															</xsl:for-each>
															<xsl:for-each select="n1:Invoice">
																<xsl:for-each select="cac:AccountingCustomerParty">
																	<xsl:for-each select="cac:Party">
																		<xsl:for-each select="cac:Contact">
																			<xsl:if test="cbc:Telephone or cbc:Telefax">
																				<tr align="left">
																					<td style="width:469px; " align="left">
																						<xsl:for-each select="cbc:Telephone">
																							<span>
																								<xsl:text>Tel: </xsl:text>
																							</span>
																							<xsl:apply-templates/>
																						</xsl:for-each>
																						<xsl:for-each select="cbc:Telefax">
																							<span>
																								<xsl:text> Fax: </xsl:text>
																							</span>
																							<xsl:apply-templates/>
																						</xsl:for-each>
																						<span>
																							<xsl:text>&#xA0;</xsl:text>
																						</span>
																					</td>
																				</tr>
																			</xsl:if>
																		</xsl:for-each>
																	</xsl:for-each>
																</xsl:for-each>
															</xsl:for-each>
															<xsl:if test="//n1:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyTaxScheme/cac:TaxScheme/cbc:Name">
																<tr align="left">
																	<td>
																		<span>
																			<xsl:text>Vergi Dairesi: </xsl:text>
																			<xsl:value-of select="//n1:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyTaxScheme/cac:TaxScheme/cbc:Name"/>
																		</span>
																	</td>
																</tr>
															</xsl:if>
															<xsl:for-each select="//n1:Invoice/cac:AccountingCustomerParty/cac:Party/cac:PartyIdentification">
																<tr align="left">
																	<td>
																		<xsl:value-of select="cbc:ID/@schemeID"/>
																		<xsl:text>: </xsl:text>
																		<xsl:value-of select="cbc:ID"/>
																	</td>
																</tr>
															</xsl:for-each>
														</tbody>
													</table>
													<hr/>
												</td>
											</tr>
										</tbody>
									</table>
									<br/>
								</td>
								<td width="60%" align="center" valign="bottom" colspan="2">
									<table border="1" height="13" id="despatchTable">
										<tbody>
											<tr>
												<td style="width:105px;" align="left">
													<span style="font-weight:bold; ">
														<xsl:text>Özelleştirme No:</xsl:text>
													</span>
												</td>
												<td style="width:110px;" align="left">
													<xsl:for-each select="n1:Invoice">
														<xsl:for-each select="cbc:CustomizationID">
															<xsl:apply-templates/>
														</xsl:for-each>
													</xsl:for-each>
												</td>
											</tr>
											<tr style="height:13px; ">
												<td align="left">
													<span style="font-weight:bold; ">
														<xsl:text>Senaryo:</xsl:text>
													</span>
												</td>
												<td align="left">
													<xsl:for-each select="n1:Invoice">
														<xsl:for-each select="cbc:ProfileID">
															<xsl:apply-templates/>
														</xsl:for-each>
													</xsl:for-each>
												</td>
											</tr>
											<tr style="height:13px; ">
												<td align="left">
													<span style="font-weight:bold; ">
														<xsl:text>Fatura Tipi:</xsl:text>
													</span>
												</td>
												<td align="left">
													<xsl:for-each select="n1:Invoice">
														<xsl:for-each select="cbc:InvoiceTypeCode">
															<xsl:apply-templates/>
														</xsl:for-each>
													</xsl:for-each>
												</td>
											</tr>
											<tr style="height:13px; ">
												<td align="left">
													<span style="font-weight:bold; ">
														<xsl:text>Fatura No:</xsl:text>
													</span>
												</td>
												<td align="left">
													<xsl:for-each select="n1:Invoice">
														<xsl:for-each select="cbc:ID">
															<xsl:apply-templates/>
														</xsl:for-each>
													</xsl:for-each>
												</td>
											</tr>
											<tr style="height:13px; ">
												<td align="left">
													<span style="font-weight:bold; ">
														<xsl:text>Fatura Tarihi:</xsl:text>
													</span>
												</td>
												<td align="left">
													<xsl:for-each select="n1:Invoice">
														<xsl:for-each select="cbc:IssueDate">
															<xsl:value-of select="substring(.,9,2)"/>-<xsl:value-of select="substring(.,6,2)"/>-<xsl:value-of select="substring(.,1,4)"/>
														</xsl:for-each>
													</xsl:for-each>
												</td>
											</tr>
<xsl:if test="n1:Invoice/cbc:IssueDate !=''">
												<tr style="height:13px; ">
													<td align="left">
														<span style="font-weight:bold; ">
															<xsl:text>Düzenlenme Tarihi</xsl:text>
														</span>
													</td>
													<td align="left">
														<xsl:for-each select="n1:Invoice">
															<xsl:for-each select="cbc:IssueDate">
																<xsl:value-of select="substring(.,9,2)"/>-<xsl:value-of select="substring(.,6,2)"/>-<xsl:value-of select="substring(.,1,4)"/>
															</xsl:for-each>
														</xsl:for-each>
													</td>
												</tr>
											</xsl:if>
											<xsl:if test="n1:Invoice/cbc:IssueTime !=''">
												<tr style="height:13px; ">
													<td align="left">
														<span style="font-weight:bold; ">
															<xsl:text>Düzenlenme Saati</xsl:text>
														</span>
													</td>
													<td align="left">
														<xsl:for-each select="n1:Invoice">
															<xsl:value-of select="substring(cbc:IssueTime,1,8)"/>
														</xsl:for-each>
													</td>
												</tr>
											</xsl:if>
											<xsl:for-each select="n1:Invoice/cac:DespatchDocumentReference">
												<tr style="height:13px; ">
													<td align="left">
														<span style="font-weight:bold; ">
															<xsl:text>İrsaliye No:</xsl:text>
														</span>
														<span>
															<xsl:text>&#xA0;</xsl:text>
														</span>
													</td>
													<td align="left">
														<xsl:value-of select="cbc:ID"/>
													</td>
												</tr>
												<tr style="height:13px; ">
													<td align="left">
														<span style="font-weight:bold; ">
															<xsl:text>İrsaliye Tarihi:</xsl:text>
														</span>
													</td>
													<td align="left">
														<xsl:for-each select="cbc:IssueDate">
															<xsl:value-of select="substring(.,9,2)"/>-<xsl:value-of select="substring(.,6,2)"/>-<xsl:value-of select="substring(.,1,4)"/>
														</xsl:for-each>
													</td>
												</tr>
											</xsl:for-each>
											<xsl:if test="//n1:Invoice/cac:OrderReference">
												<tr style="height:13px">
													<td align="left">
														<span style="font-weight:bold; ">
															<xsl:text>Sipariş No:</xsl:text>
														</span>
													</td>
													<td align="left">
														<xsl:for-each select="n1:Invoice/cac:OrderReference">
															<xsl:for-each select="cbc:ID">
																<xsl:apply-templates/>
															</xsl:for-each>
														</xsl:for-each>
													</td>
												</tr>
											</xsl:if>
											<xsl:if test="//n1:Invoice/cac:OrderReference/cbc:IssueDate">
												<tr style="height:13px">
													<td align="left">
														<span style="font-weight:bold; ">
															<xsl:text>Sipariş Tarihi:</xsl:text>
														</span>
													</td>
													<td align="left">
														<xsl:for-each select="n1:Invoice/cac:OrderReference">
															<xsl:for-each select="cbc:IssueDate">
																<xsl:value-of select="substring(.,9,2)"/>-<xsl:value-of select="substring(.,6,2)"/>-<xsl:value-of select="substring(.,1,4)"/>
															</xsl:for-each>
														</xsl:for-each>
													</td>
												</tr>
											</xsl:if>
										</tbody>
									</table>
								</td>
							</tr>
							<tr align="left">
								<table id="ettnTable">
									<tr style="height:13px;">
										<td align="left" valign="top">
											<span style="font-weight:bold; ">
												<xsl:text>ETTN:</xsl:text>
											</span>
										</td>
										<td align="left" width="240px">
											<xsl:for-each select="n1:Invoice">
												<xsl:for-each select="cbc:UUID">
													<xsl:apply-templates/>
												</xsl:for-each>
											</xsl:for-each>
										</td>
									</tr>
								</table>
							</tr>
						</tbody>
					</table>
					<div id="lineTableAligner">
						<span>
							<xsl:text>&#xA0;</xsl:text>
						</span>
					</div>
					<table border="1" id="lineTable" width="800">
						<tbody>
							<tr id="lineTableTr">
								<td id="lineTableTd" style="width:3%">
									<span style="font-weight:bold; " align="center">
										<xsl:text>Sıra No</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:15%" align="center">
									<span style="font-weight:bold; ">
										<xsl:text>Mal Hizmet</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:7.4%" align="center">
									<span style="font-weight:bold;">
										<xsl:text>Miktar</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:14%" align="center">
									<span style="font-weight:bold; ">
										<xsl:text>Birim Fiyat</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:7%" align="center">
									<span style="font-weight:bold; ">
										<xsl:text>İskonto Oranı</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:9%" align="center">
									<span style="font-weight:bold; ">
										<xsl:text>İskonto Tutarı</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:7%" align="center">
									<span style="font-weight:bold; ">
										<xsl:text>KDV Oranı</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:10%" align="center">
									<span style="font-weight:bold; ">
										<xsl:text>KDV Tutarı</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:17%; " align="center">
									<span style="font-weight:bold; ">
										<xsl:text>Diğer Vergiler</xsl:text>
									</span>
								</td>
								<td id="lineTableTd" style="width:10.6%" align="center">
									<span style="font-weight:bold; ">
										<xsl:text>Mal Hizmet Tutarı</xsl:text>
									</span>
								</td>
							</tr>
							<xsl:for-each select="//n1:Invoice/cac:InvoiceLine">
								<xsl:choose>
									<xsl:when test=".">
										<xsl:apply-templates select="."/>
									</xsl:when>
									<xsl:otherwise>
										<xsl:apply-templates select="//n1:Invoice"/>
									</xsl:otherwise>
								</xsl:choose>
							</xsl:for-each>
						</tbody>
					</table>
				</xsl:for-each>
				<table id="budgetContainerTable" width="800px">
					<tr id="budgetContainerTr" align="right">
						<td id="budgetContainerDummyTd"/>
						<td id="lineTableBudgetTd" align="right" width="200px">
							<span style="font-weight:bold; ">
								<xsl:text>Mal Hizmet Toplam Tutarı</xsl:text>
							</span>
						</td>
						<td id="lineTableBudgetTd" style="width:81px; " align="right">
							<span>
								<xsl:for-each select="n1:Invoice/cac:LegalMonetaryTotal/cbc:LineExtensionAmount">
									<xsl:call-template name="Curr_Type">
										<xsl:with-param name="valuePath" select="."/>
										<xsl:with-param name="format" select="'###.##0,00'"/>
									</xsl:call-template>
								</xsl:for-each>
							</span>
						</td>
					</tr>
					<tr id="budgetContainerTr" align="right">
						<td id="budgetContainerDummyTd"/>
						<td id="lineTableBudgetTd" align="right" width="200px">
							<span style="font-weight:bold; ">
								<xsl:text>Toplam İskonto</xsl:text>
							</span>
						</td>
						<td id="lineTableBudgetTd" style="width:81px; " align="right">
							<span>
								<xsl:for-each select="n1:Invoice/cac:LegalMonetaryTotal/cbc:AllowanceTotalAmount">
									<xsl:call-template name="Curr_Type">
										<xsl:with-param name="valuePath" select="."/>
										<xsl:with-param name="format" select="'###.##0,00'"/>
									</xsl:call-template>
								</xsl:for-each>
							</span>
						</td>
					</tr>
					<xsl:for-each select="n1:Invoice/cac:TaxTotal/cac:TaxSubtotal">
						<tr id="budgetContainerTr" align="right">
							<td id="budgetContainerDummyTd"/>
							<td id="lineTableBudgetTd" width="211px" align="right">
								<span style="font-weight:bold; ">
									<xsl:text>Hesaplanan </xsl:text>
									<xsl:value-of select="cac:TaxCategory/cac:TaxScheme/cbc:Name"/>
									<xsl:text>(%</xsl:text>
									<xsl:value-of select="cbc:Percent"/>
									<xsl:text>)</xsl:text>
								</span>
							</td>
							<td id="lineTableBudgetTd" style="width:82px; " align="right">
								<xsl:for-each select="cac:TaxCategory/cac:TaxScheme">
									<xsl:text> </xsl:text>
									<xsl:call-template name="Curr_Type">
										<xsl:with-param name="valuePath" select="../../cbc:TaxAmount"/>
										<xsl:with-param name="format" select="'###.##0,00'"/>
									</xsl:call-template>
								</xsl:for-each>
							</td>
						</tr>
					</xsl:for-each>
					<xsl:for-each select="n1:Invoice/cac:WithholdingTaxTotal/cac:TaxSubtotal">
						<xsl:if test="cbc:TaxAmount != ''">
							<tr id="budgetContainerTr" align="right">
								<td id="budgetContainerDummyTd"/>
								<td id="lineTableBudgetTd" width="211px" align="right">
									<span style="font-weight:bold; ">
										<xsl:text>KDV Tevkifat-[</xsl:text>
										<xsl:value-of select="cac:TaxCategory/cac:TaxScheme/cbc:TaxTypeCode"/>
										<xsl:text>]-</xsl:text>
										<xsl:text>(%</xsl:text>
										<xsl:value-of select="cbc:Percent"/>
										<xsl:text>)</xsl:text>
									</span>
								</td>
								<td id="lineTableBudgetTd" style="width:82px; " align="right">
									<xsl:for-each select="cac:TaxCategory/cac:TaxScheme">
										<xsl:text> </xsl:text>
										<xsl:call-template name="Curr_Type">
											<xsl:with-param name="valuePath" select="../../cbc:TaxAmount"/>
											<xsl:with-param name="format" select="'###.##0,00'"/>
										</xsl:call-template>
									</xsl:for-each>
								</td>
							</tr>
						</xsl:if>
					</xsl:for-each>
					<tr id="budgetContainerTr" align="right">
						<td id="budgetContainerDummyTd"/>
						<td id="lineTableBudgetTd" width="200px" align="right">
							<span style="font-weight:bold; ">
								<xsl:text>Vergiler Dahil Toplam Tutar</xsl:text>
							</span>
						</td>
						<td id="lineTableBudgetTd" style="width:82px; " align="right">
							<xsl:for-each select="n1:Invoice">
								<xsl:for-each select="cac:LegalMonetaryTotal">
									<xsl:for-each select="cbc:TaxInclusiveAmount">
										<xsl:call-template name="Curr_Type">
											<xsl:with-param name="valuePath" select="."/>
											<xsl:with-param name="format" select="'###.##0,00'"/>
										</xsl:call-template>
									</xsl:for-each>
								</xsl:for-each>
							</xsl:for-each>
						</td>
					</tr>
					<tr id="budgetContainerTr" align="right">
						<td id="budgetContainerDummyTd"/>
						<td id="lineTableBudgetTd" width="200px" align="right">
							<span style="font-weight:bold; ">
								<xsl:text>Ödenecek Tutar</xsl:text>
							</span>
						</td>
						<td id="lineTableBudgetTd" style="width:82px; " align="right">
							<xsl:for-each select="n1:Invoice/cac:LegalMonetaryTotal/cbc:PayableAmount">
								<xsl:call-template name="Curr_Type">
									<xsl:with-param name="valuePath" select="."/>
									<xsl:with-param name="format" select="'###.##0,00'"/>
								</xsl:call-template>
							</xsl:for-each>
						</td>
					</tr>
					<xsl:if test="//n1:Invoice/cbc:DocumentCurrencyCode != 'TRY'">
						<tr id="budgetContainerTr" align="right">
							<td id="budgetContainerDummyTd"/>
							<td id="lineTableBudgetTd" align="right" width="200px">
								<span style="font-weight:bold; ">
									<xsl:text>Toplam İskonto (TL)</xsl:text>
								</span>
							</td>
							<td id="lineTableBudgetTd" style="width:81px; " align="right">
								<span>
									<xsl:value-of select="format-number(//n1:Invoice/cac:LegalMonetaryTotal/cbc:AllowanceTotalAmount * //n1:Invoice/cac:PricingExchangeRate/cbc:CalculationRate, '###.##0,00', 'european')"/>
									<xsl:text> TL</xsl:text>
								</span>
							</td>
						</tr>
					</xsl:if>
					<xsl:for-each select="n1:Invoice/cac:TaxTotal/cac:TaxSubtotal">
						<xsl:if test="//n1:Invoice/cbc:DocumentCurrencyCode != 'TRY'">
							<tr align="right">
								<td/>
								<td id="lineTableBudgetTd" align="right" width="200px">
									<span style="font-weight:bold; ">
										<xsl:text>Hesaplanan </xsl:text>
										<xsl:value-of select="cac:TaxCategory/cac:TaxScheme/cbc:Name"/>
										<xsl:text>(%</xsl:text>
										<xsl:value-of select="cbc:Percent"/>
										<xsl:text>) (TL)</xsl:text>
									</span>
								</td>
								<td id="lineTableBudgetTd" style="width:81px; " align="right">
									<span>
										<xsl:value-of select="format-number(cbc:TaxAmount * //n1:Invoice/cac:PricingExchangeRate/cbc:CalculationRate, '###.##0,00', 'european')"/>
										<xsl:text> TL</xsl:text>
									</span>
								</td>
							</tr>
						</xsl:if>
					</xsl:for-each>
					<xsl:for-each select="n1:Invoice/cac:WithholdingTaxTotal/cac:TaxSubtotal">
						<xsl:if test="//n1:Invoice/cbc:DocumentCurrencyCode != 'TRY' and cbc:TaxAmount != ''">
							<tr id="budgetContainerTr" align="right">
								<td/>
								<td id="lineTableBudgetTd" width="211px" align="right">
									<span style="font-weight:bold; ">
										<xsl:text>KDV Tevkifat-[</xsl:text>
										<xsl:value-of select="cac:TaxCategory/cac:TaxScheme/cbc:TaxTypeCode"/>
										<xsl:text>]-</xsl:text>
										<xsl:text>(%</xsl:text>
										<xsl:value-of select="cbc:Percent"/>
										<xsl:text>) (TL)</xsl:text>
									</span>
								</td>
								<td id="lineTableBudgetTd" style="width:82px; " align="right">
									<xsl:for-each select="cac:TaxCategory/cac:TaxScheme">
										<xsl:text> </xsl:text>
										<xsl:value-of select="format-number(../../cbc:TaxAmount * //n1:Invoice/cac:PricingExchangeRate/cbc:CalculationRate, '###.##0,00', 'european')"/>
										<xsl:text> TL</xsl:text>
									</xsl:for-each>
								</td>
							</tr>
						</xsl:if>
					</xsl:for-each>
					<xsl:if test="//n1:Invoice/cbc:DocumentCurrencyCode != 'TRY'">
						<tr align="right">
							<td/>
							<td id="lineTableBudgetTd" align="right" width="200px">
								<span style="font-weight:bold; ">
									<xsl:text>Mal Hizmet Toplam Tutarı(TL)</xsl:text>
								</span>
							</td>
							<td id="lineTableBudgetTd" style="width:81px; " align="right">
								<span>
									<xsl:value-of select="format-number(//n1:Invoice/cac:LegalMonetaryTotal/cbc:LineExtensionAmount * //n1:Invoice/cac:PricingExchangeRate/cbc:CalculationRate, '###.##0,00', 'european')"/>
									<xsl:text> TL</xsl:text>
								</span>
							</td>
						</tr>
						<tr id="budgetContainerTr" align="right">
							<td/>
							<td id="lineTableBudgetTd" width="200px" align="right">
								<span style="font-weight:bold; ">
									<xsl:text>Vergiler Dahil Toplam Tutar(TL)</xsl:text>
								</span>
							</td>
							<td id="lineTableBudgetTd" style="width:82px; " align="right">
								<xsl:value-of select="format-number(//n1:Invoice/cac:LegalMonetaryTotal/cbc:TaxInclusiveAmount * //n1:Invoice/cac:PricingExchangeRate/cbc:CalculationRate, '###.##0,00', 'european')"/>
								<xsl:text> TL</xsl:text>
							</td>
						</tr>
						<tr align="right">
							<td/>
							<td id="lineTableBudgetTd" width="200px" align="right">
								<span style="font-weight:bold; ">
									<xsl:text>Ödenecek Tutar(TL)</xsl:text>
								</span>
							</td>
							<td id="lineTableBudgetTd" style="width:82px; " align="right">
								<xsl:value-of select="format-number(//n1:Invoice/cac:LegalMonetaryTotal/cbc:PayableAmount * //n1:Invoice/cac:PricingExchangeRate/cbc:CalculationRate, '###.##0,00', 'european')"/>
								<xsl:text> TL</xsl:text>
							</td>
						</tr>
					</xsl:if>
				</table>
				<br/>
				<table id="notesTable" width="800" align="left" height="100">
					<tbody>
						<tr align="left">
							<td id="notesTableTd">
								<xsl:for-each select="//n1:Invoice/cac:TaxTotal/cac:TaxSubtotal">
									<xsl:if test="cbc:Percent=0 and cac:TaxCategory/cac:TaxScheme/cbc:TaxTypeCode='0015'">
										<b>&#xA0;&#xA0;&#xA0;&#xA0;&#xA0; Vergi
											İstisna Muafiyet Sebebi:</b>
										<xsl:value-of select="cac:TaxCategory/cbc:TaxExemptionReason"/>
										<br/>
									</xsl:if>
								</xsl:for-each>
								<xsl:for-each select="n1:Invoice/cbc:Note">
									<xsl:value-of select="."/>
									<br/>
								</xsl:for-each>
								<xsl:if test="//n1:Invoice/cac:PaymentMeans/cbc:InstructionNote">
									<b>&#xA0;&#xA0;&#xA0;&#xA0;&#xA0; Ödeme
                                                                                                                                                             Notu:</b>
									<xsl:value-of select="//n1:Invoice/cac:PaymentMeans/cbc:InstructionNote"/>
									<br/>
								</xsl:if>
								<xsl:if test="//n1:Invoice/cac:PaymentMeans/cac:PayeeFinancialAccount/cbc:PaymentNote">
									<b>&#xA0;&#xA0;&#xA0;&#xA0;&#xA0; Hesap
                                                                                                                                                             Açıklaması:</b>
									<xsl:value-of select="//n1:Invoice/cac:PaymentMeans/cac:PayeeFinancialAccount/cbc:PaymentNote"/>
									<br/>
								</xsl:if>
								<xsl:if test="//n1:Invoice/cac:PaymentTerms/cbc:Note!=''">
									<b>&#xA0;&#xA0;&#xA0;&#xA0;&#xA0; Ödeme
                                                                                                                                                             Koşulu:</b>
									<xsl:value-of select="//n1:Invoice/cac:PaymentTerms/cbc:Note"/>
									<br/>
								</xsl:if>
							</td>
						</tr>
					</tbody>
				</table>				
			</body>
		</html>
	</xsl:template>
	<xsl:template match="dateFormatter">
		<xsl:value-of select="substring(.,9,2)"/>-<xsl:value-of select="substring(.,6,2)"/>-<xsl:value-of select="substring(.,1,4)"/>
	</xsl:template>
	<xsl:template match="//n1:Invoice/cac:InvoiceLine">
		<tr id="lineTableTr">
			<td id="lineTableTd">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:value-of select="./cbc:ID"/>
				</span>
			</td>
			<td id="lineTableTd">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:value-of select="./cac:Item/cbc:Name"/>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:value-of select="format-number(./cbc:InvoicedQuantity, '###.##0,####', 'european')"/>
					<xsl:if test="./cbc:InvoicedQuantity/@unitCode">
						<xsl:for-each select="./cbc:InvoicedQuantity">
							<xsl:text/>
							<xsl:choose>
								<xsl:when test="@unitCode  = 'C62'">
									<span>
										<xsl:text> Adet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'NIU'">
									<span>
										<xsl:text> Adet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CPR'">
									<span>
										<xsl:text> Adet-Çift</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'AS'">
									<span>
										<xsl:text> Asorti</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MON'">
									<span>
										<xsl:text> Ay</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'FOT'">
									<span>
										<xsl:text> Ayak</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'D92'">
									<span>
										<xsl:text> Bant</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BAR'">
									<span>
										<xsl:text> Bar</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BR'">
									<span>
										<xsl:text> Bar</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BAS'">
									<span>
										<xsl:text> Bas</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'EA'">
									<span>
										<xsl:text> Beher</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = '2W'">
									<span>
										<xsl:text> Bidon</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'T3'">
									<span>
										<xsl:text> Bin Adet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'TWH'">
									<span>
										<xsl:text> Bin KWH</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'D40'">
									<span>
										<xsl:text> Bin Lt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'R9'">
									<span>
										<xsl:text> Bin M³</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = '4A'">
									<span>
										<xsl:text> Bobin</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CL'">
									<span>
										<xsl:text> Bobin</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'E4'">
									<span>
										<xsl:text> Brüt KG</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GD'">
									<span>
										<xsl:text> Brüt Varil</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GRO'">
									<span>
										<xsl:text> Brüt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'AD'">
									<span>
										<xsl:text> Byte</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CGM'">
									<span>
										<xsl:text> Cgm</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CLT'">
									<span>
										<xsl:text> CLt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CMT'">
									<span>
										<xsl:text> CM</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CMK'">
									<span>
										<xsl:text> CM²</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CMQ'">
									<span>
										<xsl:text> CM³</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'PR'">
									<span>
										<xsl:text> Çift</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'HA'">
									<span>
										<xsl:text> Çile</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'RD'">
									<span>
										<xsl:text> Çubuk</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'SA'">
									<span>
										<xsl:text> Çuval</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'D79'">
									<span>
										<xsl:text> Demet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'A49'">
									<span>
										<xsl:text> Denye</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DC'">
									<span>
										<xsl:text> Disk</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'D61'">
									<span>
										<xsl:text> DK</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MIN'">
									<span>
										<xsl:text> DK</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DLT'">
									<span>
										<xsl:text> DLt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DMT'">
									<span>
										<xsl:text> DM</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DMK'">
									<span>
										<xsl:text> DM²</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DPC'">
									<span>
										<xsl:text> Düzine</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DPR'">
									<span>
										<xsl:text> Düzine</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DRL'">
									<span>
										<xsl:text> Düzine</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DZN'">
									<span>
										<xsl:text> Düzine</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DZP'">
									<span>
										<xsl:text> Düzine</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'Z3'">
									<span>
										<xsl:text> Fıçı</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BH'">
									<span>
										<xsl:text> Fırça</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GB'">
									<span>
										<xsl:text> Galon</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GLI'">
									<span>
										<xsl:text> Galon</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GLL'">
									<span>
										<xsl:text> Galon</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GRM'">
									<span>
										<xsl:text> Gr</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GN'">
									<span>
										<xsl:text> Gross Galon</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'GT'">
									<span>
										<xsl:text> Gross Ton</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = '10'">
									<span>
										<xsl:text> Grup</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DAY'">
									<span>
										<xsl:text> Gün</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'RG'">
									<span>
										<xsl:text> Halka</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'INH'">
									<span>
										<xsl:text> İnç</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = '4B'">
									<span>
										<xsl:text> Kap</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'NCR'">
									<span>
										<xsl:text> Karat</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'Z2'">
									<span>
										<xsl:text> Kasa/Sandık</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'D66'">
									<span>
										<xsl:text> Kaset</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = '2P'">
									<span>
										<xsl:text> Kilobyte</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'KGM'">
									<span>
										<xsl:text> Kilogram</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'KWT'">
									<span>
										<xsl:text> Kilowatt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'IE'">
									<span>
										<xsl:text> Kişi</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'KJO'">
									<span>
										<xsl:text> KJO</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'K6'">
									<span>
										<xsl:text> KLT</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'KTM'">
									<span>
										<xsl:text> Km</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'KMK'">
									<span>
										<xsl:text> Km²</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'AB'">
									<span>
										<xsl:text> Koli</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CT'">
									<span>
										<xsl:text> Koli</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CH'">
									<span>
										<xsl:text> Konteyner</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BJ'">
									<span>
										<xsl:text> Kova</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'PL'">
									<span>
										<xsl:text> Kova</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CU'">
									<span>
										<xsl:text> Kupa</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BX'">
									<span>
										<xsl:text> Kutu</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CA'">
									<span>
										<xsl:text> Kutu</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CS'">
									<span>
										<xsl:text> Kutu</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'B5'">
									<span>
										<xsl:text> Kütük</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'B55'">
									<span>
										<xsl:text> KVM</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'KWH'">
									<span>
										<xsl:text> KWH</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'LTR'">
									<span>
										<xsl:text> Litre</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'RL'">
									<span>
										<xsl:text> Makara</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'SO'">
									<span>
										<xsl:text> Makara</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MAW'">
									<span>
										<xsl:text> Megawatt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MGM'">
									<span>
										<xsl:text> MGM</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = '77'">
									<span>
										<xsl:text> Miliinç</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MLT'">
									<span>
										<xsl:text> MLt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MMT'">
									<span>
										<xsl:text> Mm</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MMQ'">
									<span>
										<xsl:text> Mm³</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MTR'">
									<span>
										<xsl:text> Mt</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MTK'">
									<span>
										<xsl:text> Mt²</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MTQ'">
									<span>
										<xsl:text> Mt³</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'MWH'">
									<span>
										<xsl:text> MWH</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'NT'">
									<span>
										<xsl:text> Net Ton</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'PA'">
									<span>
										<xsl:text> Paket</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'D97'">
									<span>
										<xsl:text> Palet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'PF'">
									<span>
										<xsl:text> Palet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BD'">
									<span>
										<xsl:text> Pano</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'PG'">
									<span>
										<xsl:text> Plaka</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'RO'">
									<span>
										<xsl:text> Rulo</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'HUR'">
									<span>
										<xsl:text> Saat</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'ST'">
									<span>
										<xsl:text> Sayfa</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BK'">
									<span>
										<xsl:text> Sepet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'SET'">
									<span>
										<xsl:text> Set</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'CY'">
									<span>
										<xsl:text> Silindir</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'D62'">
									<span>
										<xsl:text> Sn</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BO'">
									<span>
										<xsl:text> Şişe</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'LR'">
									<span>
										<xsl:text> Tabaka</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'TN'">
									<span>
										<xsl:text> Teneke</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = '26'">
									<span>
										<xsl:text> Ton</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'AA'">
									<span>
										<xsl:text> Top</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BG'">
									<span>
										<xsl:text> Torba/Poşet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'TU'">
									<span>
										<xsl:text> Tüp</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BLD'">
									<span>
										<xsl:text> Varil</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'BLL'">
									<span>
										<xsl:text> Varil</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'DR'">
									<span>
										<xsl:text> Varil</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'ANN'">
									<span>
										<xsl:text> Yıl</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'H62'">
									<span>
										<xsl:text> Yüz Adet</xsl:text>
									</span>
								</xsl:when>
								<xsl:when test="@unitCode  = 'EV'">
									<span>
										<xsl:text> Zarf</xsl:text>
									</span>
								</xsl:when>
							</xsl:choose>
						</xsl:for-each>
					</xsl:if>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:call-template name="Curr_Type">
						<xsl:with-param name="valuePath" select="./cac:Price/cbc:PriceAmount"/>
						<xsl:with-param name="format" select="'###.##0,00000000'"/>
					</xsl:call-template>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:if test="./cac:AllowanceCharge[1]/cbc:MultiplierFactorNumeric">
						<xsl:text> %</xsl:text>
						<xsl:value-of select="format-number(./cac:AllowanceCharge[1]/cbc:MultiplierFactorNumeric * 100, '###.##0,00', 'european')"/>
					</xsl:if>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:if test="./cac:AllowanceCharge[1]">
						<!--<xsl:if test="./cac:AllowanceCharge[1]/cbc:ChargeIndicator = true() ">+</xsl:if>
						<xsl:if test="./cac:AllowanceCharge[1]/cbc:ChargeIndicator = false() ">-</xsl:if>-->
						<xsl:call-template name="Curr_Type">
							<xsl:with-param name="valuePath" select="./cac:AllowanceCharge[1]/cbc:Amount"/>
							<xsl:with-param name="format" select="'###.##0,00'"/>
						</xsl:call-template>
					</xsl:if>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:for-each select="./cac:TaxTotal/cac:TaxSubtotal/cac:TaxCategory/cac:TaxScheme">
						<xsl:if test="cbc:TaxTypeCode='0015' ">
							<xsl:text/>
							<xsl:if test="../../cbc:Percent">
								<xsl:text> %</xsl:text>
								<xsl:value-of select="format-number(../../cbc:Percent, '###.##0,00', 'european')"/>
							</xsl:if>
						</xsl:if>
					</xsl:for-each>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:for-each select="./cac:TaxTotal/cac:TaxSubtotal/cac:TaxCategory/cac:TaxScheme">
						<xsl:if test="cbc:TaxTypeCode='0015' ">
							<xsl:text/>
							<xsl:call-template name="Curr_Type">
								<xsl:with-param name="valuePath" select="../../cbc:TaxAmount"/>
								<xsl:with-param name="format" select="'###.##0,00'"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:for-each>
				</span>
			</td>
			<td id="lineTableTd" style="font-size: xx-small" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:for-each select="./cac:TaxTotal/cac:TaxSubtotal/cac:TaxCategory/cac:TaxScheme">
						<xsl:if test="cbc:TaxTypeCode!='0015' ">
							<xsl:text/>
							<xsl:value-of select="cbc:Name"/>
							<xsl:if test="../../cbc:Percent">
								<xsl:text> (%</xsl:text>
								<xsl:value-of select="format-number(../../cbc:Percent, '###.##0,00', 'european')"/>
								<xsl:text>)=</xsl:text>
							</xsl:if>
							<xsl:call-template name="Curr_Type">
								<xsl:with-param name="valuePath" select="../../cbc:TaxAmount"/>
								<xsl:with-param name="format" select="'###.##0,00'"/>
							</xsl:call-template>
						</xsl:if>
					</xsl:for-each>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
					<xsl:call-template name="Curr_Type">
						<xsl:with-param name="valuePath" select="./cbc:LineExtensionAmount"/>
						<xsl:with-param name="format" select="'###.##0,00'"/>
					</xsl:call-template>
				</span>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="//n1:Invoice">
		<tr id="lineTableTr">
			<td id="lineTableTd">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
			<td id="lineTableTd" align="right">
				<span>
					<xsl:text>&#xA0;</xsl:text>
				</span>
			</td>
		</tr>
	</xsl:template>
	<xsl:template name="Curr_Type">
		<xsl:param name="format"/>
		<xsl:param name="valuePath"/>
		<xsl:value-of select="format-number($valuePath, $format, 'european')"/>
		<xsl:if test="$valuePath/@currencyID">
			<xsl:text> </xsl:text>
			<xsl:choose>
				<xsl:when test="$valuePath/@currencyID = 'TRL' or $valuePath/@currencyID = 'TRY'">
					<xsl:text>TL</xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$valuePath/@currencyID"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>